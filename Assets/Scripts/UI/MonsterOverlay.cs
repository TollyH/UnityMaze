using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MonsterOverlay : MonoBehaviour
{
    public float StartDistance = 2f;
    public float EndDistance = 0.05f;

    public float TravelDuration = 0.15f;

    private float time = 0;

    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    [SerializeField]
    private Image monsterFace;
    [SerializeField]
    private GameObject hintText;
    [SerializeField]
    private GameObject background;

    private ControlMap inputActions;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
        inputActions = new ControlMap();
    }

    private void OnEnable()
    {
        inputActions.PlayerMovement.Enable();
        time = 0;
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;

        monsterFace.rectTransform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);

        if (XRSettings.enabled)
        {
            hintText.SetActive(false);
            background.SetActive(false);

            time += Time.deltaTime / TravelDuration;
            monsterFace.rectTransform.localPosition = new Vector3(
                monsterFace.rectTransform.localPosition.x, monsterFace.rectTransform.localPosition.y,
                Mathf.Lerp((StartDistance - thisCanvas.planeDistance) * thisScaler.referenceResolution.x,
                    (EndDistance - thisCanvas.planeDistance) * thisScaler.referenceResolution.x, time));
        }
        else
        {
            hintText.SetActive(true);
            background.SetActive(true);
        }
    }

    private void OnEscapeMonster()
    {
        LevelManager.Instance.MonsterManager.EscapeMonsterClick();
    }
}
