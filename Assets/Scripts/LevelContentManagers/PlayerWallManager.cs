using UnityEngine;
using UnityEngine.XR;

public class PlayerWallManager : LevelContentManager
{
    public Vector2? WallPosition => WallTimeRemaining > 0 ? wallPosition : null;
    private Vector2 wallPosition;

    public float PlayerWallTime = 15;
    public float PlayerWallCooldown = 20;

    public float WallTimeRemaining { get; private set; } = 0;
    public float WallCooldownRemaining { get; private set; } = 0;

    private Material[] wallMaterials;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject mapContainer;
    [SerializeField]
    private VRHand leftHand;

    [SerializeField]
    private AudioSource wallSound;
    private AudioClip[] wallSoundClips;

    private void Awake()
    {
        wallMaterials = Resources.LoadAll<Material>("Materials/PlayerWall");
        wallSoundClips = Resources.LoadAll<AudioClip>("Sounds/wall_place");
    }

    private void Update()
    {
        if (!levelManager.PlayerManager.HasMovedThisLevel
            || levelManager.IsGameOver
            || levelManager.IsPaused)
        {
            return;
        }

        if (WallCooldownRemaining > 0)
        {
            WallCooldownRemaining -= Time.deltaTime;
            if (WallCooldownRemaining <= 0)
            {
                WallCooldownRemaining = 0;
            }
        }
        else if (WallTimeRemaining > 0)
        {
            WallTimeRemaining -= Time.deltaTime;
            if (WallTimeRemaining <= 0)
            {
                gameObject.DestroyAllChildren();
                WallTimeRemaining = 0;
                WallCooldownRemaining = PlayerWallCooldown;
            }
            else
            {
                // Select appropriate player wall texture depending on how long the wall has left until breaking
                Material wallMaterial = wallMaterials[
                    (int)((PlayerWallTime - WallTimeRemaining) / PlayerWallTime * wallMaterials.Length)];

                MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer renderer in meshRenderers)
                {
                    renderer.material = wallMaterial;
                }
            }
        }
    }

    private void OnPlaceWall()
    {
        float handUpProduct = Vector3.Dot(leftHand.transform.up, Vector3.up);
        if (WallCooldownRemaining > 0 || WallTimeRemaining > 0
            || !levelManager.PlayerManager.HasMovedThisLevel
            || levelManager.MonsterManager.IsPlayerStruggling
            || levelManager.IsGameOver
            || levelManager.IsPaused
            || mapContainer.activeSelf
            // Wall action is only if hand is facing outwards
            || (XRSettings.enabled &&
                (handUpProduct < -leftHand.ThreewaySelectionCrossover 
                || handUpProduct > leftHand.ThreewaySelectionCrossover)))
        {
            return;
        }

        float unitSize = levelManager.UnitSize;

        // Get the angle the camera is facing, then round it to the closest 90deg
        float roundedYaw = Mathf.Round(Camera.main.transform.eulerAngles.y / 90) * 90;
        Vector2 wallVector = roundedYaw switch
        {
            0 => new Vector2(0, 1),
            360 => new Vector2(0, 1),
            -360 => new Vector2(0, 1),
            90 => new Vector2(-1, 0),
            -270 => new Vector2(-1, 0),
            180 => new Vector2(0, -1),
            -180 => new Vector2(0, -1),
            270 => new Vector2(1, 0),
            -90 => new Vector2(1, 0),
            _ => new Vector2(0, 0)
        };

        wallPosition = levelManager.PlayerManager.GridPosition + wallVector;

        if ((wallVector.x == 0 && wallVector.y == 0) || !levelManager.CurrentLevel.IsCoordInBounds(wallPosition)
            || (levelManager.CurrentLevel[wallPosition].Wall != null))
        {
            return;
        }

        GameObject newWall = new($"PlayerWall");
        newWall.transform.parent = transform;
        newWall.transform.position = new Vector3(unitSize * -wallPosition.x, unitSize / 2, unitSize * wallPosition.y);

        GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newPlane.name = "NorthPlane";
        newPlane.transform.parent = newWall.transform;
        newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
        newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
        newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);
        newPlane.GetComponent<MeshRenderer>().material = wallMaterials[0];

        newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newPlane.name = "EastPlane";
        newPlane.transform.parent = newWall.transform;
        newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
        newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
        newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);
        newPlane.GetComponent<MeshRenderer>().material = wallMaterials[0];

        newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newPlane.name = "SouthPlane";
        newPlane.transform.parent = newWall.transform;
        newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
        newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
        newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);
        newPlane.GetComponent<MeshRenderer>().material = wallMaterials[0];

        newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newPlane.name = "WestPlane";
        newPlane.transform.parent = newWall.transform;
        newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
        newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
        newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
        newPlane.GetComponent<MeshRenderer>().material = wallMaterials[0];

        WallTimeRemaining = PlayerWallTime;
        wallSound.PlayOneShot(wallSoundClips[Random.Range(0, wallSoundClips.Length)]);
    }

    public override void OnLevelLoad(Level level)
    {
        WallTimeRemaining = 0;
        WallCooldownRemaining = 0;
        gameObject.DestroyAllChildren();
    }
}
