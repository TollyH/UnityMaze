using UnityEngine;

public class KeysManager : LevelContentManager
{
    public bool AllKeysCollected => transform.childCount == 0;

    [SerializeField]
    private GameObject collectibleSpritePrefab;

    public void ReloadKeys(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        gameObject.DestroyAllChildren();

        foreach (Vector2 coord in level.ExitKeys)
        {
            GameObject keySprite = Instantiate(
                collectibleSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            keySprite.name = $"Key{coord.x}-{coord.y}Sprite";
            keySprite.transform.parent = transform;
            BoxCollider spriteCollider = keySprite.GetComponentInChildren<BoxCollider>();
            spriteCollider.size = new Vector3(unitSize, unitSize, unitSize);
            spriteCollider.center = new Vector3(0, unitSize / 2, 0);
            keySprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/key");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadKeys(level);
    }
}