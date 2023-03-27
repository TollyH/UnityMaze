using UnityEngine;

public class MonsterManager : LevelContentManager
{
    public Vector2? GridPosition { get; private set; }
    public float? TimeToSpawn { get; private set; }

    private Renderer thisRenderer;

    private void Awake()
    {
        thisRenderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (TimeToSpawn == null || GridPosition == null)
        {
            return;
        }
        if (TimeToSpawn > 0)
        {
            TimeToSpawn -= Time.deltaTime;
            if (TimeToSpawn <= 0)
            {
                thisRenderer.enabled = true;
            }
            return;
        }
        float unitSize = LevelManager.Instance.UnitSize;
        transform.localScale = new Vector3(unitSize, unitSize, unitSize);
        transform.position = new Vector3(GridPosition.Value.x * -unitSize, 0, GridPosition.Value.y * unitSize);
    }

    private void OnSpriteTrigger(GameObject triggerObject)
    {
        if (!thisRenderer.enabled)
        {
            return;
        }
        Debug.Log("Monster hit!");
    }

    public override void OnLevelLoad(Level level)
    {
        GridPosition = level.MonsterStart;
        TimeToSpawn = level.MonsterWait;
        thisRenderer.enabled = false;
    }
}
