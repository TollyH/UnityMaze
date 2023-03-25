using System;
using UnityEngine;
using UnityEngine.XR;

public class VRHand : MonoBehaviour
{
    private ControlMap inputActions;
    private Renderer thisRenderer;
    public bool IsRightHand;

    [NonSerialized]
    public Vector3 PosOffset = new();
    [NonSerialized]
    public float YawOffset = 0;

    private void Awake()
    {
        inputActions = new ControlMap();
        thisRenderer = GetComponent<Renderer>();
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
    }
}
