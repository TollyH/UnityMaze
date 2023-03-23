using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    private readonly float sensitivity = 0.2f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float turnValue = -400 * sensitivity * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + turnValue, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            float turnValue = 400 * sensitivity * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + turnValue, 0);
        }
    }

    private void OnCameraLook(InputValue value)
    {
        float turnValue = value.Get<Vector2>().x * sensitivity;
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + turnValue, 0);
    }

    private void OnLockMouse()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
