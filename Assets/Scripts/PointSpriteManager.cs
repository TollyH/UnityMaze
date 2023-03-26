using UnityEngine;

public class PointSpriteManager : LevelContentManager
{
    [SerializeField]
    private GameObject spritePrefab;

    public void ReloadPointSprites(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;

        // Delete all previous sprites
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        GameObject startPointSprite = Instantiate(
            spritePrefab, new Vector3(level.StartPoint.x * -unitSize, 0, level.StartPoint.y * unitSize), Quaternion.identity);
        startPointSprite.name = "StartPointSprite";
        startPointSprite.transform.parent = transform;
        startPointSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/start_point");

        GameObject exitPointSprite = Instantiate(
            spritePrefab, new Vector3(level.EndPoint.x * -unitSize, 0, level.EndPoint.y * unitSize), Quaternion.identity);
        exitPointSprite.name = "ExitPointSprite";
        exitPointSprite.transform.parent = transform;
        exitPointSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/end_point");

        foreach ((Vector2 coords, string texture) in level.Decorations)
        {
            GameObject decorationSprite = Instantiate(
                spritePrefab, new Vector3(coords.x * -unitSize, 0, coords.y * unitSize), Quaternion.identity);
            decorationSprite.name = $"Decoration{coords.x}-{coords.y}Sprite";
            decorationSprite.transform.parent = transform;
            decorationSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Textures/sprite/decoration/{texture}");
        }

        if (level.MonsterStart != null)
        {
            GameObject monsterStartSprite = Instantiate(
                spritePrefab, new Vector3(level.MonsterStart.Value.x * -unitSize, 0, level.MonsterStart.Value.y * unitSize), Quaternion.identity);
            monsterStartSprite.name = "MonsterStartSprite";
            monsterStartSprite.transform.parent = transform;
            monsterStartSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/monster_spawn");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadPointSprites(level);
    }
}
