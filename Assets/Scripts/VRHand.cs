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
    private GameObject mapContainer;

    private void Awake()
    {
        inputActions = new ControlMap();
        thisRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        inputActions.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    private void Update()
    {
        thisRenderer.enabled = XRSettings.enabled;
        if (!thisRenderer.enabled)
        {
            return;
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

        if (LevelManager.Instance.IsPaused && IsRightHand)
        {
            thisRenderer.sprite = upProduct > 0 ? resetSprite : unpauseSprite;
        }
        else if (!IsRightHand && LevelManager.Instance.PlayerManager.HasMovedThisLevel
            && !LevelManager.Instance.MonsterManager.IsPlayerStruggling
            && !LevelManager.Instance.IsGameOver
            && !LevelManager.Instance.IsPaused
            && !mapContainer.activeSelf)
        {
            thisRenderer.sprite = upProduct > ThreewaySelectionCrossover ? flagSprite :
                upProduct < -ThreewaySelectionCrossover ? pauseSprite : wallSprite;
        }
        else
        {
            thisRenderer.sprite = null;
        }
    }
}
