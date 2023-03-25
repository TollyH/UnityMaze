using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRLook : MonoBehaviour
{
    private ControlMap inputActions;
    public Vector3 PosOffset { get; private set; } = new();
    public float YawOffset { get; private set; } = 0;
    public List<GameObject> Hands = new();

    private void Awake()
    {
        inputActions = new ControlMap();
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
        if (!XRSettings.enabled)
        {
            return;
        }
        Vector3 hmdPos = Quaternion.AngleAxis(-YawOffset, Vector3.up) * (inputActions.PlayerMovement.CameraMoveVR.ReadValue<Vector3>() - PosOffset);
        Quaternion hmdRot = Quaternion.AngleAxis(-YawOffset, Vector3.up) * inputActions.PlayerMovement.CameraLookVR.ReadValue<Quaternion>();

        Camera.main.transform.SetLocalPositionAndRotation(hmdPos, hmdRot);

        if (inputActions.PlayerMovement.ResetVR.IsPressed())
        {
            Vector3 rawHmdPos = inputActions.PlayerMovement.CameraMoveVR.ReadValue<Vector3>();
            PosOffset = new Vector3(rawHmdPos.x, 0, rawHmdPos.z);
            YawOffset = inputActions.PlayerMovement.CameraLookVR.ReadValue<Quaternion>().eulerAngles.y;

            foreach (GameObject hand in Hands)
            {
                VRHand handComponent = hand.GetComponent<VRHand>();
                handComponent.PosOffset = PosOffset;
                handComponent.YawOffset = YawOffset;
            }
        }
    }
}
