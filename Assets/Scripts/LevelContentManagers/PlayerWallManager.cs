using UnityEngine;

public class PlayerWallManager : LevelContentManager
{
    public Vector2? WallPosition => wallTimeRemaining > 0 ? wallPosition : null;
    private Vector2 wallPosition;

    public float PlayerWallTime = 15;
    public float PlayerWallCooldown = 20;

    private float wallTimeRemaining = 0;
    private float wallCooldownRemaining = 0;

    private Material[] wallMaterials;

    [SerializeField]
    private GameObject mapContainer;

    private ControlMap inputActions;

    private void Awake()
    {
        inputActions = new ControlMap();
        wallMaterials = Resources.LoadAll<Material>("Materials/PlayerWall");
    }

    private void OnEnable()
    {
        inputActions.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerMovement.Disable();
    }

    private void Update()
    {
        if (wallCooldownRemaining > 0)
        {
            wallCooldownRemaining -= Time.deltaTime;
            if (wallCooldownRemaining <= 0)
            {
                wallCooldownRemaining = 0;
            }
        }
        else if (wallTimeRemaining > 0)
        {
            wallTimeRemaining -= Time.deltaTime;
            if (wallTimeRemaining <= 0)
            {
                gameObject.DestroyAllChildren();
                wallTimeRemaining = 0;
                wallCooldownRemaining = PlayerWallCooldown;
            }
            else
            {
                // Select appropriate player wall texture depending on how long the wall has left until breaking
                Material wallMaterial = wallMaterials[
                    (int)((PlayerWallTime - wallTimeRemaining) / PlayerWallTime * wallMaterials.Length)];

                MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in meshRenderers)
                {
                    renderer.material = wallMaterial;
                }
            }
        }
    }

    private void OnPlaceWall()
    {
        if (wallCooldownRemaining > 0 || wallTimeRemaining > 0
            || !LevelManager.Instance.PlayerManager.HasMovedThisLevel
            || mapContainer.activeSelf)
        {
            return;
        }

        float unitSize = LevelManager.Instance.UnitSize;

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

        wallPosition = LevelManager.Instance.PlayerManager.GridPosition + wallVector;

        if ((wallVector.x == 0 && wallVector.y == 0)
            || (LevelManager.Instance.CurrentLevel[wallPosition].Wall != null))
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

        wallTimeRemaining = PlayerWallTime;
    }

    public override void OnLevelLoad(Level level)
    {
        gameObject.DestroyAllChildren();
    }
}
