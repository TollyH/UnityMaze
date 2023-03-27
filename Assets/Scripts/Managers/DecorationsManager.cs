using UnityEngine;

public class DecorationsManager : LevelContentManager
{
    [SerializeField]
    private GameObject spritePrefab;
    [SerializeField]
    private GameObject triggerSpritePrefab;

    public void ReloadDecorations(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        gameObject.DestroyAllChildren();

        foreach ((Vector2 coords, string texture) in level.Decorations)
        {
            GameObject decorationSprite = Instantiate(
                spritePrefab, new Vector3(coords.x * -unitSize, 0, coords.y * unitSize), Quaternion.identity);
            decorationSprite.name = $"Decoration{coords.x}-{coords.y}Sprite";
            decorationSprite.transform.parent = transform;
            decorationSprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
            decorationSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Textures/sprite/decoration/{texture}");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadDecorations(level);
    }
}
