using UnityEngine;

public class PickupsManager : LevelContentManager
{
    [SerializeField]
    private GameObject collectibleSpritePrefab;
    [SerializeField]
    private GameObject triggerSpritePrefab;
    [SerializeField]
    private ViewportFlash viewportFlash;

    [SerializeField]
    private AudioSource gunPickup;
    [SerializeField]
    private AudioSource sensorPickup;

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
            keySensorSprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
            keySensorSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/key_sensor");
        }

        foreach (Vector2 coord in level.Guns)
        {
            GameObject gunSprite = Instantiate(
                triggerSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            gunSprite.name = $"Gun{coord.x}-{coord.y}Sprite";
            gunSprite.transform.parent = transform;
            gunSprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
            gunSprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/gun");
        }
    }

    public void OnCollect(GameObject collectedObject)
    {
        PlayerManager player = LevelManager.Instance.PlayerManager;
        if (collectedObject.name.StartsWith("KeySensor"))
        {
            sensorPickup.Play();
            player.RemainingKeySensorTime = player.KeySensorTime;
            viewportFlash.PerformFlash(Colors.White, 0.4f, 0.4f);
        }
    }

    public void OnSpriteTrigger(GameObject collectedObject)
    {
        PlayerManager player = LevelManager.Instance.PlayerManager;
        if (collectedObject.name.StartsWith("Gun"))
        {
            // Player already has a gun, it shouldn't be collected
            if (player.HasGun)
            {
                return;
            }
            gunPickup.Play();
            player.HasGun = true;
            Destroy(collectedObject);
            viewportFlash.PerformFlash(Colors.White, 0.4f, 0.4f);
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadPickups(level);
    }
}
