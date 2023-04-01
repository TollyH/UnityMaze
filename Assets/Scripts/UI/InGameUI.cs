using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class InGameUI : MonoBehaviour
{
    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    private ControlMap inputActions;

    [SerializeField]
    private Image statsPanel;
    [SerializeField]
    private Image controlsPanel;
    [SerializeField]
    private Image gunControlPanel;
    [SerializeField]
    private Image outerCompass;
    [SerializeField]
    private TextMeshProUGUI keysLabel;
    [SerializeField]
    private TextMeshProUGUI movesLabel;
    [SerializeField]
    private TextMeshProUGUI timeLabel;

    [SerializeField]
    private RectTransform compassNeedle;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
        inputActions = new ControlMap();
    }

    private void OnEnable()
    {
        inputActions.UIControl.Enable();
    }

    private void OnDisable()
    {
        inputActions.UIControl.Disable();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;

        KeysManager keys = LevelManager.Instance.KeysManager;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;

        keysLabel.text = $"Keys: {keys.TotalLevelKeys - keys.KeysRemaining}/{keys.TotalLevelKeys}";
        movesLabel.text = $"Moves: {player.LevelMoves:0.0}";
        timeLabel.text = $"Time: {player.LevelTime:0.0}";

        Color bgColor = monster.IsMonsterSpawned ? Color.red : Color.black;
        bgColor.a = 0.5f;
        statsPanel.color = bgColor;
        controlsPanel.color = bgColor;
        gunControlPanel.color = bgColor;

        if (monster.IsMonsterSpawned)
        {
            compassNeedle.gameObject.SetActive(true);
            Vector3 monsterDirection = player.transform.position - monster.transform.position;
            float yawOffset = Mathf.Atan2(monsterDirection.x, monsterDirection.z) * Mathf.Rad2Deg;
            compassNeedle.rotation = Quaternion.Euler(0f, 0f, 180 - (yawOffset - Camera.main.transform.rotation.eulerAngles.y));
        }
        else
        {
            compassNeedle.gameObject.SetActive(false);
        }
    }

    private void OnToggleCompass()
    {
        outerCompass.gameObject.SetActive(!outerCompass.gameObject.activeSelf);
    }

    private void OnToggleStats()
    {
        bool newState = !statsPanel.gameObject.activeSelf;
        statsPanel.gameObject.SetActive(newState);
        controlsPanel.gameObject.SetActive(newState);
        // TODO: Toggle with player's possession of a gun
        gunControlPanel.gameObject.SetActive(newState);
    }
}
