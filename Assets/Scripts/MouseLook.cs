using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private ControlMap inputActions;
    public float Sensitivity;

    [SerializeField]
    private GameObject mapContainer;

    private void Start()
    {
        inputActions = LevelManager.Instance.InputActions;
    }

    private void Update()
    {
        if (LevelManager.Instance.IsGameOver || mapContainer.activeSelf
            || LevelManager.Instance.IsPaused)
        {
            return;
        }
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
        if (LevelManager.Instance.PlayerManager.HasGun && Cursor.lockState == CursorLockMode.Locked)
        {
            return;
        }
        Vector3 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (view.x >= 0 && view.x <= 1 && view.y >= 0 && view.y <= 1)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
