using System;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : LevelContentManager
{
    public Vector2 MazePosition => new((-transform.position.x + (LevelManager.Instance.UnitSize / 2)) / LevelManager.Instance.UnitSize,
            (transform.position.z + (LevelManager.Instance.UnitSize / 2)) / LevelManager.Instance.UnitSize);
    public Vector2 GridPosition => new((int)MazePosition.x, (int)MazePosition.y);

    public bool HasMovedThisLevel { get; private set; }

    public float LevelTime { get; private set; }
    public float LevelMoves { get; private set; }

    public float KeySensorTime = 10;
    [NonSerialized]
    public float RemainingKeySensorTime = 0;

    [NonSerialized]
    public bool HasGun = false;

    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (HasMovedThisLevel)
        {
            LevelTime += Time.deltaTime;
            RemainingKeySensorTime -= Time.deltaTime;
            if (RemainingKeySensorTime < 0)
            {
                RemainingKeySensorTime = 0;
            }
        }
    }

    public override void OnLevelLoad(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        LevelTime = 0;
        LevelMoves = 0;
        HasMovedThisLevel = false;
        RemainingKeySensorTime = 0;
        HasGun = false;

        // Initialise player position, place them in the middle of the square
        Vector2 startPos = level.StartPoint * unitSize;
        characterController.MoveAbsolute(new Vector3(-startPos.x, transform.position.y, startPos.y));
        if (!XRSettings.enabled)
        {
            characterController.height = unitSize / 2;
            capsuleCollider.height = unitSize / 2;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, unitSize / 2, Camera.main.transform.position.z),
                Quaternion.Euler(0, 0, 0));
        }
        characterController.center = new Vector3(0, characterController.height / 2, 0);
        capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
    }

    private void OnMove(float distance)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        HasMovedThisLevel = true;
        LevelMoves += distance / unitSize;
    }
}
