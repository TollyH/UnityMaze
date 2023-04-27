using UnityEngine;
using UnityEngine.XR;

public class PlayerActions : MonoBehaviour
{
    private PlayerManager player;

    [SerializeField]
    private AudioSource gunfire;
    [SerializeField]
    private AudioSource vrGunfire;

    [SerializeField]
    private GameObject barrelStart;

    private void Start()
    {
        player = LevelManager.Instance.PlayerManager;
    }

    private void OnFireGun()
    {
        if (!player.HasGun
            || LevelManager.Instance.MonsterManager.IsPlayerStruggling
            || LevelManager.Instance.IsGameOver
            || LevelManager.Instance.IsPaused
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
            Debug.DrawLine(position, hit.point, Colors.Green, 500);
            if (hit.collider.transform == LevelManager.Instance.MonsterManager.transform)
            {
                LevelManager.Instance.MonsterManager.KillMonster();
            }
        }
    }
}
