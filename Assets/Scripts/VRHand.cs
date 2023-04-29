using System;
using UnityEngine;
using UnityEngine.XR;

public class VRHand : MonoBehaviour
{
    private ControlMap inputActions;
    private SpriteRenderer thisRenderer;
    public bool IsRightHand;

    public float ThreewaySelectionCrossover = 0.6f;

    [NonSerialized]
    public Vector3 PosOffset = new();
    [NonSerialized]
    public float YawOffset = 0;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private Sprite resetSprite;
    [SerializeField]
    private Sprite unpauseSprite;
    [SerializeField]
    private Sprite pauseSprite;
    [SerializeField]
    private Sprite wallSprite;
    [SerializeField]
    private Sprite flagSprite;

    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject tracer;
    [SerializeField]
    private Animator gunAnimator;

    [SerializeField]
    private GameObject mapContainer;

    private void Awake()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        inputActions = levelManager.InputActions;
    }

    private void Update()
    {
        thisRenderer.enabled = XRSettings.enabled;
        if (gun != null)
        {
            bool levelActive = !levelManager.MonsterManager.IsPlayerStruggling
                && !levelManager.IsGameOver
                && !levelManager.IsPaused;
            gun.SetActive(XRSettings.enabled && levelActive);
            tracer.SetActive(XRSettings.enabled && levelManager.PlayerManager.HasGun && levelActive);
        }
        if (!thisRenderer.enabled)
        {
            return;
        }
        else if (gunAnimator != null && gun.activeSelf)
        {
            gunAnimator.Play(levelManager.PlayerManager.HasGun ? "Closed" : "Opened", 0);
        }

        Vector3 handPos = IsRightHand
            ? inputActions.PlayerMovement.RightHandPosVR.ReadValue<Vector3>()
            : inputActions.PlayerMovement.LeftHandPosVR.ReadValue<Vector3>();
        handPos -= PosOffset;
        handPos = Quaternion.AngleAxis(-YawOffset, Vector3.up) * handPos;
        Quaternion handRot = IsRightHand
            ? inputActions.PlayerMovement.RightHandRotVR.ReadValue<Quaternion>()
            : inputActions.PlayerMovement.LeftHandRotVR.ReadValue<Quaternion>();
        handRot = Quaternion.AngleAxis(-YawOffset, Vector3.up) * handRot;
        transform.SetLocalPositionAndRotation(handPos, handRot);

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        float upProduct = Vector3.Dot(transform.up, Vector3.up);

        if (levelManager.IsPaused && IsRightHand)
        {
            thisRenderer.sprite = upProduct > 0 ? resetSprite : unpauseSprite;
        }
        else if (!IsRightHand && levelManager.PlayerManager.HasMovedThisLevel
            && !levelManager.MonsterManager.IsPlayerStruggling
            && !levelManager.IsGameOver
            && !levelManager.IsPaused
            && !mapContainer.activeSelf)
        {
            thisRenderer.sprite = upProduct > ThreewaySelectionCrossover ? flagSprite :
                upProduct < -ThreewaySelectionCrossover ? pauseSprite : wallSprite;
        }
        else if (levelManager.IsGameOver && !levelManager.IsPaused && !IsRightHand)
        {
            thisRenderer.sprite = pauseSprite;
        }
        else
        {
            thisRenderer.sprite = null;
        }
    }
}
