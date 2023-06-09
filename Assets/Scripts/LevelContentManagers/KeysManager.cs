using System.Collections.Generic;
using UnityEngine;

public class KeysManager : LevelContentManager
{
    public bool AllKeysCollected => transform.childCount == 0;
    public int KeysRemaining => transform.childCount;
    public int TotalLevelKeys { get; private set; }

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject collectibleSpritePrefab;
    [SerializeField]
    private ViewportFlash viewportFlash;

    [SerializeField]
    private AudioSource keySound;
    private AudioClip[] keySoundClips;

    private void Awake()
    {
        keySoundClips = Resources.LoadAll<AudioClip>("Sounds/key_pickup");
    }

    public void ReloadKeys(Level level)
    {
        float unitSize = levelManager.UnitSize;
        gameObject.DestroyAllChildren();

        foreach (Vector2 coord in level.ExitKeys)
        {
            GameObject keySprite = Instantiate(
                collectibleSpritePrefab, new Vector3(coord.x * -unitSize, 0, coord.y * unitSize), Quaternion.identity);
            keySprite.name = $"Key{coord.x}-{coord.y}Sprite";
            keySprite.transform.parent = transform;
            keySprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
            keySprite.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/sprite/key");
            keySprite.GetComponent<CollectibleSprite>().levelManager = levelManager;
        }
        TotalLevelKeys = transform.childCount;
    }

    public HashSet<Vector2> GetRemainingKeyCoords()
    {
        float unitSize = levelManager.UnitSize;
        HashSet<Vector2> keyCoords = new(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform key = transform.GetChild(i);
            Vector3 coord = key.localPosition;
            _ = keyCoords.Add(new Vector2(-coord.x / unitSize, coord.z / unitSize));
        }
        return keyCoords;
    }

    public void OnCollect(GameObject _)
    {
        viewportFlash.PerformFlash(Colors.White, 0.4f, 0.4f);

        keySound.PlayOneShot(keySoundClips[Random.Range(0, keySoundClips.Length)]);
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadKeys(level);
    }
}
