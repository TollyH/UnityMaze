using UnityEngine;
using UnityEngine.XR;

public class FlagManager : LevelContentManager
{
    [SerializeField]
    private LevelManager levelManager;

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

    public bool IsFlagged(Vector2 coord, out GameObject flag)
    {
        float unitSize = levelManager.UnitSize;
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
        if (!levelManager.PlayerManager.HasMovedThisLevel
            || levelManager.MonsterManager.IsPlayerStruggling
            || levelManager.IsGameOver
            || levelManager.IsPaused
            || levelManager.IsMulti
            || mapContainer.activeSelf
            // Flag action is only if hand is facing downwards
            || (XRSettings.enabled && handUpProduct < leftHand.ThreewaySelectionCrossover))
        {
            return;
        }
        float unitSize = levelManager.UnitSize;
        Vector2 coord = levelManager.PlayerManager.GridPosition;
        Vector3 pos = new(coord.x * -unitSize, 0, coord.y * unitSize);

        if (IsFlagged(coord, out GameObject flag))
        {
            Destroy(flag);
            return;
        }

        GameObject flagSprite = Instantiate(
                flagSpritePrefab, pos, Quaternion.identity);
        flagSprite.name = $"Key{coord.x}-{coord.y}Sprite";
        flagSprite.transform.parent = transform;
        flagSprite.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
        flagSprite.GetComponent<FlagSprite>().levelManager = levelManager;

        flagSound.PlayOneShot(flagSoundClips[Random.Range(0, flagSoundClips.Length)]);
    }

    public override void OnLevelLoad(Level level)
    {
        gameObject.DestroyAllChildren();
    }
}
