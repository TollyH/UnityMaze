using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private ControlMap inputActions;

    public int CurrentLevelIndex { get; private set; }

    public Level[] LoadedLevels { get; private set; }

    [SerializeField]
    private float unitSize = 4f;

    private readonly Dictionary<string, Material> loadedWallMaterials = new();
    private Material missingMaterial;

    private void Awake()
    {
        // LevelManager is a singleton
        if (Instance == null)
        {
            Instance = this;
            inputActions = new ControlMap();
            missingMaterial = Resources.Load<Material>("Materials/Missing");
            DontDestroyOnLoad(gameObject);
            LoadLevelJson(Path.Join(Application.streamingAssetsPath, "maze_levels.json"));
            ReloadWallTextures();
            SetSkyTexture(Path.Join(Application.streamingAssetsPath, "textures", "sky.png"));
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
        GameObject wallsContainer = GameObject.Find("MazeWalls");
        // Delete all previous walls
        while (wallsContainer.transform.childCount > 0)
        {
            DestroyImmediate(wallsContainer.transform.GetChild(0).gameObject);
        }

        // Initialise player position, place them in the middle of the square
        Vector2 startPos = (level.StartPoint * unitSize) + new Vector2(0.5f, 0.5f);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<CharacterController>().MoveAbsolute(new Vector3(-startPos.x, player.transform.position.y, startPos.y));
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Create walls and collision
        for (int x = 0; x < level.Dimensions.x; x++)
        {
            for (int y = 0; y < level.Dimensions.y; y++)
            {
                Level.GridSquareContents contents = level[x, y];
                if (contents.Wall != null)
                {
                    GameObject newWall = new($"MazeWall{x}-{y}");
                    newWall.transform.parent = wallsContainer.transform;
                    newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * y);

                    GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "NorthPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(contents.Wall.Value.Item3, missingMaterial);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "EastPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);
                    newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(contents.Wall.Value.Item4, missingMaterial);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "SouthPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);
                    newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(contents.Wall.Value.Item1, missingMaterial);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "WestPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
                    newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(contents.Wall.Value.Item2, missingMaterial);
                }

                if (contents.PlayerCollide)
                {
                    GameObject newWallCollision = new($"MazeWallCollide{x}-{y}");
                    newWallCollision.transform.parent = wallsContainer.transform;
                    newWallCollision.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * y);

                    GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "NorthPlane";
                    // Colliders may or may not be visible
                    Destroy(newPlane.GetComponent<Renderer>());
                    newPlane.transform.parent = newWallCollision.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "EastPlane";
                    // Colliders may or may not be visible
                    Destroy(newPlane.GetComponent<Renderer>());
                    newPlane.transform.parent = newWallCollision.transform;
                    newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "SouthPlane";
                    // Colliders may or may not be visible
                    Destroy(newPlane.GetComponent<Renderer>());
                    newPlane.transform.parent = newWallCollision.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "WestPlane";
                    // Colliders may or may not be visible
                    Destroy(newPlane.GetComponent<Renderer>());
                    newPlane.transform.parent = newWallCollision.transform;
                    newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
                }
            }
        }

        // Create maze edge
        for (int x = 0; x < level.Dimensions.x; x++)
        {
            GameObject newWall = new($"MazeWallNorthEdge{x}");
            newWall.transform.parent = wallsContainer.transform;
            newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * level.Dimensions.y);

            GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "SouthPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);
            newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(level.EdgeWallTextureName, missingMaterial);

            newWall = new($"MazeWallSouthEdge{x}");
            newWall.transform.parent = wallsContainer.transform;
            newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, -1 * unitSize);

            newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "NorthPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);
            newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(level.EdgeWallTextureName, missingMaterial);
        }
        for (int y = 0; y < level.Dimensions.y; y++)
        {
            GameObject newWall = new($"MazeWallEastEdge{y}");
            newWall.transform.parent = wallsContainer.transform;
            newWall.transform.position = new Vector3(unitSize, unitSize / 2, unitSize * y);

            GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "WestPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
            newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(level.EdgeWallTextureName, missingMaterial);

            newWall = new($"MazeWallWestEdge{y}");
            newWall.transform.parent = wallsContainer.transform;
            newWall.transform.position = new Vector3(-level.Dimensions.x * unitSize, unitSize / 2, unitSize * y);

            newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "EastPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);
            newPlane.GetComponent<MeshRenderer>().material = loadedWallMaterials.GetValueOrDefault(level.EdgeWallTextureName, missingMaterial);
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

    public void ReloadWallTextures()
    {
        loadedWallMaterials.Clear();
        foreach (string file in Directory.GetFiles(Path.Join(Application.streamingAssetsPath, "textures", "wall"), "*.png"))
        {
            Texture2D newTex = new(128, 128, TextureFormat.RGBA32, false);
            _ = newTex.LoadImage(File.ReadAllBytes(file));
            newTex.filterMode = FilterMode.Point;
            newTex.Apply();
            string fileName = Path.GetFileNameWithoutExtension(file);
            loadedWallMaterials[fileName] = new Material(Shader.Find("Standard"))
            {
                mainTexture = newTex,
                shaderKeywords = new string[1] { "_SPECULARHIGHLIGHTS_OFF" }
            };
        }
    }

    public void SetSkyTexture(string path)
    {
        Texture2D newTex = new(128, 128, TextureFormat.RGBA32, false);
        _ = newTex.LoadImage(File.ReadAllBytes(path));
        newTex.Apply();
        Material sky = Resources.Load<Material>("Materials/Sky");
        sky.mainTexture = newTex;
    }
}
