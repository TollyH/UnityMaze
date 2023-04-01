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
    private GameObject mapContainer;
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

    [SerializeField]
    private GameObject mapSquarePrefab;

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

        UpdateStats();
        UpdateCompass();
        UpdateMap();
    }

    private void UpdateStats()
    {
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
    }

    private void UpdateCompass()
    {
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;

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
            remainingCompassTime = CompassTime;
            isCompassBurnedOut = false;
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

    private void UpdateMap()
    {
        mapContainer.DestroyAllChildren();
        if (!mapContainer.activeSelf)
        {
            return;
        }
        float unitSize = LevelManager.Instance.UnitSize;
        float playerGridOffset = unitSize / 2;
        Level currentLevel = LevelManager.Instance.CurrentLevel;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;
        Vector2 tileSize = new(Screen.width / currentLevel.Dimensions.x, Screen.height / currentLevel.Dimensions.y);

        for (int x = 0; x < currentLevel.Dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.Dimensions.y; y++)
            {
                Vector2 pnt = new(x, y);
                Color colour;
                if ((int)((-player.transform.position.x + playerGridOffset) / unitSize) == (int)pnt.x
                    && (int)((player.transform.position.z + playerGridOffset) / unitSize) == (int)pnt.y)
                {
                    colour = Colors.Blue;
                }
                else if (monster.IsMonsterSpawned && (int)monster.GridPosition.Value.x == (int)pnt.x && (int)monster.GridPosition.Value.y == (int)pnt.y)
                {
                    colour = Colors.DarkRed;
                }
                // TODO: Player walls
                // else if (false)
                // {
                //     colour = Colors.Purple;
                // }
                // TODO: Keys, only if key sensor is held
                // else if (false)
                // {
                //     colour = Colors.Gold;
                // }
                else if (currentLevel.MonsterStart == pnt)
                {
                    colour = Colors.DarkGreen;
                }
                // TODO: Player flags
                // else if (false)
                // {
                //     colour = Colors.LightBlue;
                // }
                else if (currentLevel.StartPoint == pnt)
                {
                    colour = Colors.Red;
                }
                else
                {
                    colour = currentLevel[pnt].Wall is null ? Colors.White : Colors.Black;
                }

                GameObject newMapSquare = Instantiate(mapSquarePrefab, mapContainer.transform, false);
                newMapSquare.name = $"MapSquare{x}-{y}";
                RectTransform rect = newMapSquare.GetComponent<RectTransform>();
                rect.sizeDelta = tileSize;
                rect.position = new Vector3(x * tileSize.x, Screen.height - (y * tileSize.y), 0);
                Image image = newMapSquare.GetComponent<Image>();
                image.color = colour;
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

    private void OnToggleMap()
    {
        mapContainer.SetActive(!mapContainer.activeSelf);
    }
}
