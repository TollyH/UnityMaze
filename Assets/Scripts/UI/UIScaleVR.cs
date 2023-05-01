using UnityEngine;
using UnityEngine.XR;

public class UIScaleVR : MonoBehaviour
{
    private ControlMap inputActions;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject leaderboard;
    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private RectTransform[] rects;

    private void Start()
    {
        inputActions = levelManager.InputActions;
    }

    private void Update()
    {
        if ((!levelManager.IsPaused && !leaderboard.activeSelf
            && !(levelManager.IsMulti && levelManager.MultiplayerManager.IsCoop && statsPanel.activeSelf))
            || !XRSettings.enabled)
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
