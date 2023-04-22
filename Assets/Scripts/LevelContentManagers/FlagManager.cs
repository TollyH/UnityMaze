using UnityEngine;

public class FlagManager : LevelContentManager
{
    private ControlMap inputActions;

    [SerializeField]
    private GameObject flagSpritePrefab;

    [SerializeField]
    private GameObject mapContainer;

    [SerializeField]
    private AudioSource flagSound;
    private AudioClip[] flagSoundClips;

    private void Awake()
    {
        inputActions = new ControlMap();
        flagSoundClips = Resources.LoadAll<AudioClip>("Sounds/flag_place");
    }

    private void OnEnable()
    {
        inputActions.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    public bool IsFlagged(Vector2 coord, out GameObject flag)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        Vector3 pos = new(coord.x * -unitSize, 0, coord.y * unitSize);
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Collider>().bounds.Contains(pos))
            {
                flag = child.gameObject;
                return true;
            }
        }
        flag = null;
        return false;
    }

    private void OnPlaceFlag()
    {
        if (!LevelManager.Instance.PlayerManager.HasMovedThisLevel || mapContainer.activeSelf)
        {
            return;
        }
        float unitSize = LevelManager.Instance.UnitSize;
        Vector2 coord = LevelManager.Instance.PlayerManager.GridPosition;
        Vector3 pos = new(coord.x * -unitSize, 0, coord.y * unitSize);

        if (IsFlagged(coord, out GameObject flag))
        {
            Destroy(flag);
            return;
        }

        GameObject keySprite = Instantiate(
                flagSpritePrefab, pos, Quaternion.identity);
        keySprite.name = $"Key{coord.x}-{coord.y}Sprite";
        keySprite.transform.parent = transform;
        keySprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);

        flagSound.PlayOneShot(flagSoundClips[Random.Range(0, flagSoundClips.Length)]);
    }

    public override void OnLevelLoad(Level level)
    {
        gameObject.DestroyAllChildren();
    }
}
