using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private ControlMap inputActions;
    public float Sensitivity;

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
        float turnValue = inputActions.PlayerMovement.CameraLook.ReadValue<Vector2>().x * Sensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + turnValue, 0);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            turnValue = inputActions.PlayerMovement.CameraLookMouse.ReadValue<Vector2>().x * Sensitivity * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + turnValue, 0);
        }
    }

    private void OnLockMouse()
    {
        Vector3 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (view.x >= 0 && view.x <= 1 && view.y >= 0 && view.y <= 1)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
