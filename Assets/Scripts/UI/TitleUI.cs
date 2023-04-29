using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class TitleUI : MonoBehaviour
{
    public static string NewPopupTitle = null;
    public static string NewPopupContent = null;

    [SerializeField]
    private GameObject popupBox;
    [SerializeField]
    private TextMeshProUGUI popupTitle;
    [SerializeField]
    private TextMeshProUGUI popupContent;

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

        if (NewPopupTitle != null && NewPopupContent != null)
        {
            popupTitle.text = NewPopupTitle;
            popupContent.text = NewPopupContent;
            NewPopupTitle = null;
            NewPopupContent = null;
            popupBox.SetActive(true);
        }
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("Scenes/MazeLevelScene");
    }

    public void OnPopupClose()
    {
        popupBox.SetActive(false);
    }
}
