using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private ControlMap inputActions;
    private readonly float sensitivity = 0.2f;

    private void Awake()
    {
        inputActions = new ControlMap();
    }

    private void OnEnable()
    {
        inputActions.gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.gameplay.Disable();
    }

    private void Update()
    {
        float turnValue = inputActions.gameplay.CameraLook.ReadValue<Vector2>().x * sensitivity;
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + turnValue, 0);
    }

    private void OnLockMouse()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
