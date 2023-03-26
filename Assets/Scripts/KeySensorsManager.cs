using UnityEngine;

public class KeySensorsManager : LevelContentManager
{
    [SerializeField]
    private GameObject collectibleSpritePrefab;

    public void ReloadKeySensors(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        foreach (Vector2 coord in level.KeySensors)
        {
            GameObject keySprite = Instantiate(
                collectibleSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            keySprite.name = $"KeySensor{coord.x}-{coord.y}Sprite";
            keySprite.transform.parent = transform;
            BoxCollider spriteCollider = keySprite.GetComponentInChildren<BoxCollider>();
            spriteCollider.size = new Vector3(unitSize, unitSize, unitSize);
            spriteCollider.center = new Vector3(0, unitSize / 2, 0);
            keySprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/key_sensor");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadKeySensors(level);
    }
}
