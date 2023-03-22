using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int CurrentLevelIndex { get; private set; }

    public Level[] LoadedLevels { get; private set; }

    private readonly float unitSize = 1.8f;

    private void Awake()
    {
        // LevelManager is a singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevelJson(Path.Join(Application.streamingAssetsPath, "maze_levels.json"));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int levelIndex)
    {
        CurrentLevelIndex = levelIndex;
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
