using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class TitleUI : MonoBehaviour
{
    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    private TitleScreenControls inputActions;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
        inputActions = new TitleScreenControls();
    }

    private void OnEnable()
    {
        inputActions.Buttons.Enable();
    }

    private void OnDisable()
    {
        inputActions.Buttons.Disable();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("Scenes/MazeLevelScene");
    }
}
