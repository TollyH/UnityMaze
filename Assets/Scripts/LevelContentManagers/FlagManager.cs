using UnityEngine;
using UnityEngine.XR;

public class FlagManager : LevelContentManager
{
    private ControlMap inputActions;

    [SerializeField]
    private GameObject flagSpritePrefab;

    [SerializeField]
    private GameObject mapContainer;
    [SerializeField]
    private VRHand leftHand;

    [SerializeField]
    private AudioSource flagSound;
    private AudioClip[] flagSoundClips;

    private void Awake()
    {
        flagSoundClips = Resources.LoadAll<AudioClip>("Sounds/flag_place");
    }

    private void Start()
    {
        inputActions = LevelManager.Instance.InputActions;
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
        float handUpProduct = Vector3.Dot(leftHand.transform.up, Vector3.up);
        if (!LevelManager.Instance.PlayerManager.HasMovedThisLevel
            || LevelManager.Instance.MonsterManager.IsPlayerStruggling
            || LevelManager.Instance.IsGameOver
            || LevelManager.Instance.IsPaused
            || mapContainer.activeSelf
            // Flag action is only if hand is facing downwards
            || (XRSettings.enabled && handUpProduct < leftHand.ThreewaySelectionCrossover))
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
