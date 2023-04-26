using System.Collections.Generic;
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
    private Image keySensorIndicator;
    [SerializeField]
    private Image gunControlPanel;
    [SerializeField]
    private Image outerCompass;
    [SerializeField]
    private GameObject mapContainer;
    [SerializeField]
    private GameObject mapSquaresContainer;
    [SerializeField]
    private TextMeshProUGUI keysLabel;
    [SerializeField]
    private TextMeshProUGUI movesLabel;
    [SerializeField]
    private TextMeshProUGUI timeLabel;
    [SerializeField]
    private GameObject deathInputHint;
    [SerializeField]
    private TextMeshProUGUI resetPrompt;


    [SerializeField]
    private Image compassTime;
    [SerializeField]
    private Image wallTime;

    [SerializeField]
    private RectTransform compassNeedle;
    [SerializeField]
    private RectTransform compassBurnIndicator;
    [SerializeField]
    private RectTransform playerDirectionIndicator;

    [SerializeField]
    private GameObject gunFirstPerson;

    [SerializeField]
    private GameObject mapSquarePrefab;

    [SerializeField]
    private AudioSource mapClosedSound;
    [SerializeField]
    private AudioSource compassClosedSound;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
        remainingCompassTime = CompassTime;
    }

    private void Start()
    {
        inputActions = LevelManager.Instance.InputActions;
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;

        gunFirstPerson.SetActive(LevelManager.Instance.PlayerManager.HasGun && !XRSettings.enabled);
        deathInputHint.SetActive(!XRSettings.enabled);

        resetPrompt.text = XRSettings.enabled ? "PAUSED" : "Press 'y' to reset or 'n' to cancel";

        UpdateStats();
        UpdateCompass();
        UpdateMap();
        UpdateTimeIndicators();
    }

    private void UpdateStats()
    {
        KeysManager keys = LevelManager.Instance.KeysManager;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;
        int currentLevelIndex = LevelManager.Instance.CurrentLevelIndex;

        if (player.HasMovedThisLevel)
        {
            keysLabel.text = $"Keys: {keys.TotalLevelKeys - keys.KeysRemaining}/{keys.TotalLevelKeys}";
            movesLabel.text = $"Moves: {player.LevelMoves:0.0}";
            timeLabel.text = $"Time: {player.LevelTime:0.0}";
        }
        else
        {
            // Show highscores if player hasn't moved yet
            keysLabel.text = $"Keys: 0/{keys.TotalLevelKeys}";
            movesLabel.text = $"Moves: {LevelManager.Instance.Highscores[currentLevelIndex].Item2:0.0}";
            timeLabel.text = $"Time: {LevelManager.Instance.Highscores[currentLevelIndex].Item1:0.0}";
        }

        Color bgColor = monster.IsMonsterSpawned ? Color.red : Color.black;
        bgColor.a = 0.5f;
        statsPanel.color = bgColor;
        controlsPanel.color = bgColor;
        gunControlPanel.color = bgColor;
        gunControlPanel.gameObject.SetActive(statsPanel.gameObject.activeSelf && player.HasGun);

        keySensorIndicator.fillAmount = player.RemainingKeySensorTime / player.KeySensorTime;

        if (XRSettings.enabled)
        {
            controlsPanel.gameObject.SetActive(false);
            gunControlPanel.gameObject.SetActive(false);
        }
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
                compassNeedle.localRotation = Quaternion.Euler(0f, 0f, 180 - (yawOffset - Camera.main.transform.rotation.eulerAngles.y));
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

        if (mapContainer.activeSelf
            || LevelManager.Instance.MonsterManager.IsPlayerStruggling
            || LevelManager.Instance.IsGameOver
            || LevelManager.Instance.IsPaused)
        {
            return;
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
        mapSquaresContainer.DestroyAllChildren();
        if (!mapContainer.activeSelf)
        {
            return;
        }
        Level currentLevel = LevelManager.Instance.CurrentLevel;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        Rect mapContainerRect = mapSquaresContainer.GetComponent<RectTransform>().rect;
        Vector2 tileSize = new(mapContainerRect.width / currentLevel.Dimensions.x, mapContainerRect.height / currentLevel.Dimensions.y);
        Vector2 playerGridPosition = player.MazePosition;
        HashSet<Vector2> keyPositions = LevelManager.Instance.KeysManager.GetRemainingKeyCoords();

        for (int x = 0; x < currentLevel.Dimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.Dimensions.y; y++)
            {
                Vector2 pnt = new(x, y);
                Color colour;
                if ((int)playerGridPosition.x == (int)pnt.x && (int)playerGridPosition.y == (int)pnt.y)
                {
                    colour = Colors.Blue;
                }
                else if (pnt == LevelManager.Instance.PlayerWallManager.WallPosition)
                {
                    colour = Colors.Purple;
                }
                else if (player.RemainingKeySensorTime > 0 && keyPositions.Contains(pnt))
                {
                    colour = Colors.Gold;
                }
                else if (currentLevel.MonsterStart == pnt)
                {
                    colour = Colors.DarkGreen;
                }
                else if (LevelManager.Instance.FlagManager.IsFlagged(pnt, out _))
                {
                    colour = Colors.LightBlue;
                }
                else if (currentLevel.StartPoint == pnt)
                {
                    colour = Colors.Red;
                }
                else
                {
                    colour = currentLevel[pnt].Wall is null ? Colors.White : Colors.Black;
                }

                GameObject newMapSquare = Instantiate(mapSquarePrefab, mapSquaresContainer.transform, false);
                newMapSquare.name = $"MapSquare{x}-{y}";
                RectTransform rect = newMapSquare.GetComponent<RectTransform>();
                rect.sizeDelta = tileSize;
                rect.localPosition = new Vector3((x * tileSize.x) - (mapContainerRect.width / 2), (mapContainerRect.height / 2) - (y * tileSize.y), 0);
                Image image = newMapSquare.GetComponent<Image>();
                image.color = colour;
            }
        }

        playerDirectionIndicator.localPosition = new Vector3(playerGridPosition.x * tileSize.x, mapContainerRect.height - (playerGridPosition.y * tileSize.y), 0);
        playerDirectionIndicator.localRotation = Quaternion.Euler(0, 0, 180 - Camera.main.transform.rotation.eulerAngles.y);
    }

    private void UpdateTimeIndicators()
    {
        compassTime.color = isCompassBurnedOut ? Colors.Red : Colors.DarkGreen;
        float compassTimeDiameter = 32 * (remainingCompassTime / CompassTime);
        compassTime.rectTransform.sizeDelta = new Vector2(compassTimeDiameter, compassTimeDiameter);

        PlayerWallManager playerWall = LevelManager.Instance.PlayerWallManager;

        wallTime.color = playerWall.WallTimeRemaining > 0 ? Colors.Red : Colors.DarkGreen;
        float wallTimeDiameter;
        if (playerWall.WallCooldownRemaining == 0 && playerWall.WallTimeRemaining == 0)
        {
            wallTimeDiameter = 32;
        }
        else
        {
            wallTimeDiameter = 32 * (playerWall.WallCooldownRemaining > 0
                ? 1 - (playerWall.WallCooldownRemaining / playerWall.PlayerWallCooldown)
                : (playerWall.WallTimeRemaining / playerWall.PlayerWallTime));
        }
        wallTime.rectTransform.sizeDelta = new Vector2(wallTimeDiameter, wallTimeDiameter);
    }

    private void OnToggleCompass()
    {
        outerCompass.gameObject.SetActive(!outerCompass.gameObject.activeSelf);
        if (!outerCompass.gameObject.activeSelf)
        {
            compassClosedSound.Play();
        }
    }

    private void OnToggleStats()
    {
        bool newState = !statsPanel.gameObject.activeSelf;
        statsPanel.gameObject.SetActive(newState);
        controlsPanel.gameObject.SetActive(newState);
    }

    private void OnToggleMap()
    {
        mapContainer.SetActive(!mapContainer.activeSelf);
        if (!mapContainer.activeSelf)
        {
            mapClosedSound.Play();
        }
    }
}
