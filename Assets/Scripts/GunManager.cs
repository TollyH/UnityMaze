using UnityEngine;

public class GunManager : LevelContentManager
{
    [SerializeField]
    private GameObject collectibleSpritePrefab;

    public void ReloadGuns(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        foreach (Vector2 coord in level.Guns)
        {
            GameObject gunSprite = Instantiate(
                collectibleSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            gunSprite.name = $"Gun{coord.x}-{coord.y}Sprite";
            gunSprite.transform.parent = transform;
            BoxCollider spriteCollider = gunSprite.GetComponentInChildren<BoxCollider>();
            spriteCollider.size = new Vector3(unitSize, unitSize, unitSize);
            spriteCollider.center = new Vector3(0, unitSize / 2, 0);
            gunSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/gun");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadGuns(level);
    }
}
