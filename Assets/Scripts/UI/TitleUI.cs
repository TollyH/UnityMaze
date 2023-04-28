using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TitleUI : MonoBehaviour
{
    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;
    }
}
