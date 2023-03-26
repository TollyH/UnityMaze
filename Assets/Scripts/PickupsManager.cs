using UnityEngine;

public class PickupsManager : LevelContentManager
{
    [SerializeField]
    private GameObject collectibleSpritePrefab;

    public void ReloadPickups(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        gameObject.DestroyAllChildren();

        foreach (Vector2 coord in level.KeySensors)
        {
            GameObject keySensorSprite = Instantiate(
                collectibleSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            keySensorSprite.name = $"KeySensor{coord.x}-{coord.y}Sprite";
            keySensorSprite.transform.parent = transform;
            BoxCollider spriteCollider = keySensorSprite.GetComponentInChildren<BoxCollider>();
            spriteCollider.size = new Vector3(unitSize, unitSize, unitSize);
            spriteCollider.center = new Vector3(0, unitSize / 2, 0);
            keySensorSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/key_sensor");
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
        ReloadPickups(level);
    }
}