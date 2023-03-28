using System;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : LevelContentManager
{
    public bool HasMovedThisLevel { get; private set; }

    [NonSerialized]
    public float LevelTime = 0;
    [NonSerialized]
    public float LevelMoves = 0;

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
        }
    }

    public override void OnLevelLoad(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        LevelTime = 0;
        LevelMoves = 0;
        HasMovedThisLevel = false;

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
