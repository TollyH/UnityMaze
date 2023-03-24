using UnityEngine;

public class VRLook : MonoBehaviour
{
    private ControlMap inputActions;
    private Vector3 posOffset = new();
    private float rotOffset = 0;

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
        Vector3 hmdPos = inputActions.PlayerMovement.CameraMoveVR.ReadValue<Vector3>() - posOffset;
        Vector3 hmdRot = inputActions.PlayerMovement.CameraLookVR.ReadValue<Quaternion>().eulerAngles;
        hmdRot = new Vector3(hmdRot.x, hmdRot.y - rotOffset, hmdRot.z);

        // Rotate position offset by rotation offset (keeps direction consistent with real-world position from HMD)
        float radians = rotOffset * Mathf.Deg2Rad;
        float sinAngle = Mathf.Sin(radians);
        float cosAngle = Mathf.Cos(radians);
        float newX = (hmdPos.x * cosAngle) - (hmdPos.z * sinAngle);
        float newZ = (hmdPos.x * sinAngle) + (hmdPos.z * cosAngle);

        Camera.main.transform.SetLocalPositionAndRotation(
            hmdPos == default ? new Vector3(0, 2, 0) : new Vector3(newX, hmdPos.y, newZ), Quaternion.Euler(hmdRot));

        if (inputActions.PlayerMovement.ResetVR.IsPressed())
        {
            posOffset = inputActions.PlayerMovement.CameraMoveVR.ReadValue<Vector3>();
            posOffset.y = 0;
            rotOffset = inputActions.PlayerMovement.CameraLookVR.ReadValue<Quaternion>().eulerAngles.y;
        }
    }
}
