using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ViewportFlash : MonoBehaviour
{
    private Canvas thisCanvas;
    private Image thisImage;

    private float duration = 1;
    private float time = 1;

    private Color startColor;
    private Color endColor;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;

        time += Time.deltaTime / duration;
        thisImage.color = Color.Lerp(startColor, endColor, time);
    }

    public void PerformFlash(Color startColor, float duration, float startAlpha = 1)
    {
        this.startColor = new(startColor.r, startColor.g, startColor.b, startAlpha);
        endColor = new(startColor.r, startColor.g, startColor.b, 0);
        this.duration = duration;

        thisImage.color = startColor;
        time = 0;
    }
}
