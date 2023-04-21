using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MonsterOverlay : MonoBehaviour
{
    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    [SerializeField]
    private Image monsterFace;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;

        monsterFace.rectTransform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }
}
