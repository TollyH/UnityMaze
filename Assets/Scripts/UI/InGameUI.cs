using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class InGameUI : MonoBehaviour
{
    public float CompassTime = 10;
    public float CompassChargeNormMultiplier = 0.5f;
    public float CompassChargeBurnMultiplier = 1.0f;
    public float CompassChargeDelay = 1.5f;

    private float remainingCompassTime;
    private float timeToCompassCharge;
    private bool isCompassBurnedOut;

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
    [SerializeField]
    private RectTransform compassBurnIndicator;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
        inputActions = new ControlMap();
        remainingCompassTime = CompassTime;
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
            if (isCompassBurnedOut)
            {
                compassNeedle.gameObject.SetActive(false);
                compassBurnIndicator.gameObject.SetActive(true);
                float compassBurnSize = 155f * ((CompassTime - remainingCompassTime) / CompassTime);
                compassBurnIndicator.sizeDelta = new Vector2(compassBurnSize, compassBurnSize);
            }
            else
            {
                compassNeedle.gameObject.SetActive(true);
                compassBurnIndicator.gameObject.SetActive(false);
                Vector3 monsterDirection = player.transform.position - monster.transform.position;
                float yawOffset = Mathf.Atan2(monsterDirection.x, monsterDirection.z) * Mathf.Rad2Deg;
                compassNeedle.rotation = Quaternion.Euler(0f, 0f, 180 - (yawOffset - Camera.main.transform.rotation.eulerAngles.y));
                compassNeedle.sizeDelta = new Vector2(5, 77.5f * (remainingCompassTime / CompassTime));
            }
        }
        else
        {
            compassNeedle.gameObject.SetActive(false);
            compassBurnIndicator.gameObject.SetActive(false);
        }

        if (outerCompass.gameObject.activeSelf && monster.IsMonsterSpawned && !isCompassBurnedOut)
        {
            remainingCompassTime -= Time.deltaTime;
            timeToCompassCharge = CompassChargeDelay;
            if (remainingCompassTime <= 0)
            {
                isCompassBurnedOut = true;
            }
        }
        else if (remainingCompassTime < CompassTime)
        {
            if (timeToCompassCharge <= 0 || isCompassBurnedOut)
            {
                float multiplier = 1 / (isCompassBurnedOut ? CompassChargeBurnMultiplier : CompassChargeNormMultiplier);
                remainingCompassTime += Time.deltaTime * multiplier;
                if (remainingCompassTime >= CompassTime)
                {
                    remainingCompassTime = CompassTime;
                    isCompassBurnedOut = false;
                }
            }
            else if (timeToCompassCharge > 0)
            {
                timeToCompassCharge -= Time.deltaTime;
            }
            
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
