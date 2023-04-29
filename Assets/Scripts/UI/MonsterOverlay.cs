using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MonsterOverlay : MonoBehaviour
{
    public float StartDistance = 2f;
    public float EndDistance = 0.25f;

    public float TravelDuration = 0.15f;

    private float time = 0;

    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private Image monsterFace;
    [SerializeField]
    private GameObject hintText;
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject uiCamera;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
    }

    private void OnEnable()
    {
        time = 0;
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;
        monsterFace.rectTransform.localPosition = Vector3.zero;

        if (XRSettings.enabled)
        {
            hintText.SetActive(false);
            background.SetActive(false);

            time += Time.deltaTime / TravelDuration;
            monsterFace.rectTransform.position = Vector3.Lerp(
                uiCamera.transform.position + (uiCamera.transform.forward * StartDistance),
                uiCamera.transform.position + (uiCamera.transform.forward * EndDistance), time);
        }
        else
        {
            hintText.SetActive(true);
            background.SetActive(true);
        }

        monsterFace.rectTransform.localPosition += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }

    private void OnEscapeMonster()
    {
        levelManager.MonsterManager.EscapeMonsterClick();
    }
}
