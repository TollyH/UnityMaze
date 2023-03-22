using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int CurrentLevelIndex { get; private set; }

    private readonly float unitSize = 1.8f;

    private void Awake()
    {
        // LevelManager is a singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
}
