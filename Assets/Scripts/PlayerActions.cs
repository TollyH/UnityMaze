using UnityEngine;
using UnityEngine.XR;

public class PlayerActions : MonoBehaviour
{
    private PlayerManager player;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private AudioSource gunfire;
    [SerializeField]
    private AudioSource vrGunfire;

    [SerializeField]
    private GameObject barrelStart;

    private void Start()
    {
        player = levelManager.PlayerManager;
    }

    private void OnFireGun()
    {
        if (!player.HasGun
            || levelManager.MonsterManager.IsPlayerStruggling
            || levelManager.IsGameOver
            || levelManager.IsPaused
            || (Cursor.lockState != CursorLockMode.Locked && !XRSettings.enabled))
        {
            return;
        }
        player.HasGun = false;

        Vector3 direction;
        Vector3 position;
        if (XRSettings.enabled)
        {
            vrGunfire.Play();
            position = barrelStart.transform.position;
            direction = barrelStart.transform.forward;
        }
        else
        {
            gunfire.Play();
            position = Camera.main.transform.position;
            direction = Camera.main.transform.forward;
        }

        if (Physics.Raycast(position, direction, out RaycastHit hit))
        {
            if (hit.collider.transform == levelManager.MonsterManager.transform)
            {
                levelManager.MonsterManager.KillMonster();
            }
        }
    }
}
