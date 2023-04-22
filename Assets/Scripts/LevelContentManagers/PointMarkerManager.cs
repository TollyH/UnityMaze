using UnityEngine;

public class PointMarkerManager : LevelContentManager
{
    [field: SerializeField]
    public GameObject StartPointSprite { get; private set; }
    [field: SerializeField]
    public GameObject ExitPointSprite { get; private set; }
    [field: SerializeField]
    public GameObject MonsterSpawnSprite { get; private set; }

    private SpriteRenderer exitPointSpriteRenderer;

    private Sprite endPoint;
    private Sprite endPointActive;

    private void Awake()
    {
        exitPointSpriteRenderer = ExitPointSprite.GetComponentInChildren<SpriteRenderer>();
        endPoint = Resources.Load<Sprite>("Textures/sprite/end_point");
        endPointActive = Resources.Load<Sprite>("Textures/sprite/end_point_active");
    }

    private void Update()
    {
        exitPointSpriteRenderer.sprite = LevelManager.Instance.KeysManager.AllKeysCollected ? endPointActive : endPoint;
    }

    public void ReloadPointMarkers(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;

        Vector3 scale = new(unitSize, unitSize, unitSize);
        StartPointSprite.transform.localScale = scale;
        ExitPointSprite.transform.localScale = scale;
        MonsterSpawnSprite.transform.localScale = scale;

        StartPointSprite.transform.position = new Vector3(level.StartPoint.x * -unitSize, 0, level.StartPoint.y * unitSize);
        ExitPointSprite.transform.position = new Vector3(level.EndPoint.x * -unitSize, 0, level.EndPoint.y * unitSize);

        if (level.MonsterStart == null)
        {
            MonsterSpawnSprite.SetActive(false);
        }
        else
        {
            MonsterSpawnSprite.SetActive(true);
            MonsterSpawnSprite.transform.position = new Vector3(level.MonsterStart.Value.x * -unitSize, 0, level.MonsterStart.Value.y * unitSize);
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadPointMarkers(level);
    }

    private void OnSpriteTrigger(GameObject triggerObject)
    {
        if (triggerObject != null && triggerObject.name == "ExitPointSprite"
            && LevelManager.Instance.KeysManager.AllKeysCollected)
        {
            LevelManager.Instance.WinLevel();
        }
    }
}
