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
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;

        time += Time.deltaTime / duration;
        thisImage.color = Color.Lerp(startColor, endColor, time);

        thisImage.enabled = time < 1;
    }

    public void PerformFlash(Color color, float duration, float startAlpha = 1, float endAlpha = 0)
    {
        startColor = new(color.r, color.g, color.b, startAlpha);
        endColor = new(color.r, color.g, color.b, endAlpha);
        this.duration = duration;

        thisImage.color = color;
        time = 0;
    }
}
