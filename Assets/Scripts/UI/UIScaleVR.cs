using UnityEngine;
using UnityEngine.XR;

public class UIScaleVR : MonoBehaviour
{
    private ControlMap inputActions;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private RectTransform[] rects;

    private void Start()
    {
        inputActions = levelManager.InputActions;
    }

    private void Update()
    {
        if (!levelManager.IsPaused || !XRSettings.enabled)
        {
            return;
        }

        float inputValue = inputActions.UIControl.UIScaleVR.ReadValue<Vector2>().y * Time.deltaTime;

        foreach (RectTransform rect in rects)
        {
            rect.localScale = new Vector3(
                rect.localScale.x + inputValue,
                rect.localScale.y + inputValue,
                rect.localScale.z);
        }
    }
}
