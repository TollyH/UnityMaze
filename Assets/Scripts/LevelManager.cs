using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private ControlMap inputActions;

    public int CurrentLevelIndex { get; private set; }

    public Level[] LoadedLevels { get; private set; }
    public Level CurrentLevel => LoadedLevels[CurrentLevelIndex];

    public float UnitSize = 4f;

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

    [SerializeField]
    private GameObject player;
    
    private LevelContentManager[] contentManagers;

    private void Awake()
    {
        // LevelManager is a singleton
        if (Instance == null)
        {
            Instance = this;
            inputActions = new ControlMap();
            contentManagers = new LevelContentManager[6]
            {
                KeysManager, DecorationsManager, WallsManager, PickupsManager, PointMarkerManager,
                MonsterManager
            };
            DontDestroyOnLoad(gameObject);
            LoadLevelJson(Path.Join(Application.streamingAssetsPath, "maze_levels.json"));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        inputActions.LevelControl.Enable();
    }

    private void OnDisable()
    {
        inputActions.LevelControl.Disable();
    }

    private void Start()
    {
        LoadLevel(0);
    }

    private void OnNextLevel()
    {
        if (CurrentLevelIndex < LoadedLevels.Length - 1)
        {
            LoadLevel(CurrentLevelIndex + 1);
        }
    }

    private void OnPreviousLevel()
    {
        if (CurrentLevelIndex > 0)
        {
            LoadLevel(CurrentLevelIndex - 1);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        CurrentLevelIndex = levelIndex;
        Level level = LoadedLevels[levelIndex];

        // Initialise player position, place them in the middle of the square
        Vector2 startPos = level.StartPoint * UnitSize;
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.MoveAbsolute(new Vector3(-startPos.x, player.transform.position.y, startPos.y));
        if (!XRSettings.enabled)
        {
            characterController.height = UnitSize / 2;
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x,UnitSize / 2, Camera.main.transform.position.z),
                Quaternion.Euler(0, 0, 0));
        }

        foreach (LevelContentManager manager in contentManagers)
        {
            manager.OnLevelLoad(level);
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
}
