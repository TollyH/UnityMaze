using UnityEngine;
using UnityEngine.XR;

public class PlayerActions : MonoBehaviour
{
    private PlayerManager player;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private ViewportFlash viewportFlash;

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

        Vector3 direction;
        Vector3 position;
        AudioSource audioSource;
        if (XRSettings.enabled)
        {
            audioSource = vrGunfire;
            position = barrelStart.transform.position;
            direction = barrelStart.transform.forward;
        }
        else
        {
            audioSource = gunfire;
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

        if (levelManager.IsMulti)
        {
            ShotResponse response = levelManager.MultiplayerManager.FireGun(position, direction);
            if (!levelManager.MultiplayerManager.IsCoop && response is ShotResponse.HitNoKill or ShotResponse.Killed)
            {
                viewportFlash.PerformFlash(Colors.White, 0.4f, 0.4f);
            }
            if (response != ShotResponse.Denied)
            {
                audioSource.Play();
            }
            if (levelManager.MultiplayerManager.IsCoop)
            {
                player.HasGun = false;
            }
        }
        else
        {
            player.HasGun = false;
            audioSource.Play();
        }
    }
}
