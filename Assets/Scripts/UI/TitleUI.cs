using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class TitleUI : MonoBehaviour
{
    /// <summary>
    /// Set this to open a popup box. Automatically set back to null.
    /// </summary>
    public static string NewPopupTitle = null;
    /// <summary>
    /// Set this to open a popup box. Automatically set back to null.
    /// </summary>
    public static string NewPopupContent = null;

    [SerializeField]
    private GameObject popupBox;
    [SerializeField]
    private TextMeshProUGUI popupTitle;
    [SerializeField]
    private TextMeshProUGUI popupContent;

    [SerializeField]
    private GameObject configBox;

    [SerializeField]
    private GameObject inputBox;
    [SerializeField]
    private TextMeshProUGUI inputTitle;
    [SerializeField]
    private TextMeshProUGUI inputContent;
    [SerializeField]
    private TMP_InputField inputText;

    private string multiplayerIP = null;
    private string multiplayerPort = null;
    private string multiplayerName = null;

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

    public void OnServerConnect()
    {
        if (inputBox.activeSelf)
        {
            // Already asking for input
            return;
        }
        inputTitle.text = "Enter Server";
        inputContent.text = "Enter the server address to connect to.\nThis should be in IP address form.";
        inputText.text = "127.0.0.1";
        inputBox.SetActive(true);
    }

    public void OnInputSubmit()
    {
        if (multiplayerIP == null)
        {
            multiplayerIP = inputText.text;
            inputTitle.text = "Enter Port";
            inputContent.text = "Enter the port number to use.\nAsk the server host if you are unsure what this is.";
            inputText.text = "13375";
        }
        else if (multiplayerPort == null)
        {
            multiplayerPort = inputText.text;
            inputTitle.text = "Enter Your Name";
            inputContent.text = "Enter the name to use.\nThere is a limit of 24 characters.";
            inputText.text = "Player";
        }
        else
        {
            multiplayerName = inputText.text;
            inputBox.SetActive(false);

            popupTitle.text = "Connecting";
            popupContent.text = "Please wait";
            popupBox.SetActive(true);

            LevelManager.NewMultiplayerServer = $"{multiplayerIP}:{multiplayerPort}";
            LevelManager.NewMultiplayerName = multiplayerName;
            OnPlay();
        }
    }

    public void OnPopupClose()
    {
        popupBox.SetActive(false);
    }

    public void OnConfigOpen()
    {
        configBox.SetActive(true);
    }
}
