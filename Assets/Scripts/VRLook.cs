using UnityEngine;

public class VRLook : MonoBehaviour
{
    private ControlMap inputActions;

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
        Vector3 hmdPos = inputActions.PlayerMovement.CameraMoveVR.ReadValue<Vector3>();
        transform.SetLocalPositionAndRotation(
            hmdPos == default ? new Vector3(0, 2, 0) : hmdPos,
            inputActions.PlayerMovement.CameraLookVR.ReadValue<Quaternion>());
    }
}
