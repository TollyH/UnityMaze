using UnityEngine;
using UnityEngine.XR;

public class PlayerActions : MonoBehaviour
{
    private ControlMap inputActions;
    private PlayerManager player;

    private void Awake()
    {
        inputActions = new ControlMap();
    }

    private void Start()
    {
        player = LevelManager.Instance.PlayerManager;
    }

    private void OnEnable()
    {
        inputActions.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    private void OnFireGun()
    {
        if (!player.HasGun
            || LevelManager.Instance.MonsterManager.IsPlayerStruggling
            || LevelManager.Instance.IsGameOver)
        {
            return;
        }
        player.HasGun = false;

        if (XRSettings.enabled)
        {
            // TODO: VR gun
        }
        else
        {
            if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit))
            {
                if (hit.collider.transform == LevelManager.Instance.MonsterManager.transform)
                {
                    LevelManager.Instance.MonsterManager.KillMonster();
                }
            }
        }
    }
}
