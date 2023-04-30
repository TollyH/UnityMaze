using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class LevelManager : MonoBehaviour
{
    public ControlMap InputActions { get; private set; }

    public int CurrentLevelIndex { get; private set; }

    public Level[] LoadedLevels { get; private set; }
    public Level CurrentLevel => LoadedLevels[CurrentLevelIndex];

    public float UnitSize = 4f;

    public float MultiplayerPingInterval = 0.04f;

    public bool IsGameOver { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;

    public bool IsMulti { get; private set; } = false;
    public Multiplayer MultiplayerManager { get; private set; }

    [field: SerializeField]
    public KeysManager KeysManager { get; private set; }
    [field: SerializeField]
    public DecorationsManager DecorationsManager { get; private set; }
    [field: SerializeField]
    public WallsManager WallsManager { get; private set; }
    [field: SerializeField]
    public PickupsManager PickupsManager { get; private set; }
    [field: SerializeField]
    public PointMarkerManager PointMarkerManager { get; private set; }
    [field: SerializeField]
    public MonsterManager MonsterManager { get; private set; }
    [field: SerializeField]
    public PlayerManager PlayerManager { get; private set; }
    [field: SerializeField]
    public PlayerWallManager PlayerWallManager { get; private set; }
    [field: SerializeField]
    public FlagManager FlagManager { get; private set; }

    public (float, float)[] Highscores { get; private set; }

    private LevelContentManager[] contentManagers;

    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerInput uiInput;
    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private GameObject victoryScreen;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private VRHand leftHand;
    [SerializeField]
    private VRHand rightHand;

    private float timeSinceServerPing = 0;

    private PlayerInput[] allInputs;

    private void Awake()
    {
        InputActions = new ControlMap();
        allInputs = FindObjectsOfType<PlayerInput>();
        contentManagers = new LevelContentManager[9]
        {
            KeysManager, DecorationsManager, WallsManager, PickupsManager, PointMarkerManager,
            MonsterManager, PlayerManager, PlayerWallManager, FlagManager
        };
        LoadLevelJson(Path.Join(Application.streamingAssetsPath, "maze_levels.json"));
    }

    private void OnEnable()
    {
        InputActions.PlayerMovement.Enable();
        InputActions.LevelControl.Enable();
        InputActions.UIControl.Enable();
    }

    private void OnDisable()
    {
        InputActions.PlayerMovement.Disable();
        InputActions.LevelControl.Disable();
        InputActions.UIControl.Disable();
    }

    private void Start()
    {
        if (!IsMulti)
        {
            LoadLevel(0);
        }
        UpdateHighscores();
    }

    private void Update()
    {
        if (IsMulti)
        {
            timeSinceServerPing += Time.deltaTime;
            if (timeSinceServerPing >= MultiplayerPingInterval)
            {
                timeSinceServerPing = 0;
                MultiplayerManager.Ping(PlayerManager.transform.position);
                if (!MultiplayerManager.IsCoop)
                {
                    if (MultiplayerManager.HitsRemaining == 0)
                    {
                        KillPlayer();
                    }
                    if (deathScreen.activeSelf && MultiplayerManager.HitsRemaining != 0)
                    {
                        // We were dead, but server has processed our respawn.
                        IsGameOver = false;
                        deathScreen.SetActive(false);
                        playerInput.enabled = true;
                        uiInput.enabled = true;
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        IsPaused = pauseScreen.activeSelf;
    }

    private void OnNextLevel()
    {
        if (CurrentLevelIndex < LoadedLevels.Length - 1 && !IsMulti)
        {
            LoadLevel(CurrentLevelIndex + 1);
        }
    }

    private void OnPreviousLevel()
    {
        if (CurrentLevelIndex > 0 && !IsMulti)
        {
            LoadLevel(CurrentLevelIndex - 1);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        CurrentLevelIndex = levelIndex;
        Level level = LoadedLevels[levelIndex];

        deathScreen.SetActive(false);
        victoryScreen.SetActive(false);
        pauseScreen.SetActive(false);
        IsGameOver = false;
        IsPaused = false;
        playerInput.enabled = true;
        uiInput.enabled = true;

        if (IsMulti)
        {
            if (!MultiplayerManager.IsCoop)
            {
                PlayerManager.RandomisePlayerCoords();
                // Remove pickups and monsters from deathmatches.
                CurrentLevel.ExitKeys.Clear();
                CurrentLevel.KeySensors.Clear();
                CurrentLevel.Guns.Clear();
                CurrentLevel.MonsterStart = null;
                CurrentLevel.MonsterWait = null;
                CurrentLevel.EndPoint = new Vector2(-1, -1);  // Make end inaccessible in deathmatches
                CurrentLevel.StartPoint = new Vector2(-1, -1);  // Hide start point in deathmatches
                PointMarkerManager.ReloadPointMarkers(CurrentLevel);
                PlayerManager.HasGun = true; // Player always has gun in deathmatch
            }
            else
            {
                // Spawn monster instantly in coop (if there is one)
                CurrentLevel.MonsterWait = CurrentLevel.MonsterStart == null ? null : 0;
            }
        }

        foreach (LevelContentManager manager in contentManagers)
        {
            manager.OnLevelLoad(level);
        }
    }

    public void KillPlayer()
    {
        deathScreen.SetActive(true);
        IsGameOver = true;
        playerInput.enabled = false;
        uiInput.enabled = false;
    }

    public void WinLevel()
    {
        victoryScreen.SetActive(true);
        IsGameOver = true;
        playerInput.enabled = false;
        uiInput.enabled = false;

        if (PlayerManager.LevelTime < Highscores[CurrentLevelIndex].Item1 || Highscores[CurrentLevelIndex].Item1 == 0)
        {
            Highscores[CurrentLevelIndex] = (PlayerManager.LevelTime, Highscores[CurrentLevelIndex].Item2);
            PlayerPrefs.SetFloat($"{CurrentLevelIndex}-time", PlayerManager.LevelTime);
        }
        if (PlayerManager.LevelMoves < Highscores[CurrentLevelIndex].Item2 || Highscores[CurrentLevelIndex].Item2 == 0)
        {
            Highscores[CurrentLevelIndex] = (Highscores[CurrentLevelIndex].Item1, PlayerManager.LevelMoves);
            PlayerPrefs.SetFloat($"{CurrentLevelIndex}-moves", PlayerManager.LevelMoves);
        }
    }

    public void UpdateHighscores()
    {
        Highscores = new (float, float)[LoadedLevels.Length];
        for (int i = 0; i < LoadedLevels.Length; i++)
        {
            Highscores[i] = (PlayerPrefs.GetFloat($"{i}-time", 0), PlayerPrefs.GetFloat($"{i}-moves", 0));
        }
    }

    /// <summary>
    /// Load and deserialize a level JSON file. The file must be a list of levels as created by the <see cref="SaveLevelJson"/> function.
    /// </summary>
    public void LoadLevelJson(string path)
    {
        List<JsonLevel> jsonLevels = JsonConvert.DeserializeObject<List<JsonLevel>>(File.ReadAllText(path));
        LoadedLevels = jsonLevels is null ? Array.Empty<Level>() : jsonLevels.Select(x => x.GetLevel()).ToArray();
    }

    /// <summary>
    /// Serialize and save the list of levels.
    /// </summary>
    public void SaveLevelJson(string path)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(LoadedLevels.Select(x => x.GetJsonLevel())));
    }

    private void OnUnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnPause()
    {
        if (IsMulti && !MultiplayerManager.IsCoop && deathScreen.activeSelf)
        {
            MultiplayerManager.Respawn();
            PlayerManager.RandomisePlayerCoords();
            return;
        }

        if (IsMulti)
        {
            return;
        }

        float handUpProduct = Vector3.Dot(leftHand.transform.up, Vector3.up);
        // Pause action is only if hand is facing upwards
        if (IsPaused || (XRSettings.enabled && handUpProduct > -leftHand.ThreewaySelectionCrossover && !IsGameOver))
        {
            return;
        }
        IsPaused = true;
        pauseScreen.SetActive(true);
        playerInput.enabled = false;
        uiInput.enabled = false;
    }

    private void OnUnpause()
    {
        if (!IsPaused)
        {
            return;
        }
        float upProduct = Vector3.Dot(rightHand.transform.up, Vector3.up);
        // When using VR, only unpause if right hand is pointing upwards
        if (XRSettings.enabled && upProduct > 0)
        {
            return;
        }

        pauseScreen.SetActive(false);
        playerInput.enabled = true;
        uiInput.enabled = true;
    }

    private void OnPauseReset()
    {
        if (!IsPaused)
        {
            return;
        }
        float upProduct = Vector3.Dot(rightHand.transform.up, Vector3.up);
        // When using VR, only reset if right hand is pointing downwards
        if (XRSettings.enabled && upProduct <= 0)
        {
            return;
        }

        LoadLevel(CurrentLevelIndex);
    }

    private void OnVRMount()
    {
        // Disable then re-enable all player inputs so that VR controllers are recognised fully
        foreach (PlayerInput input in allInputs)
        {
            input.enabled = false;
            input.enabled = true;
        }
    }
}
