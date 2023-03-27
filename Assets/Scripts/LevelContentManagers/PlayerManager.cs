using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : LevelContentManager
{
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void OnLevelLoad(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;

        // Initialise player position, place them in the middle of the square
        Vector2 startPos = level.StartPoint * unitSize;
        characterController.MoveAbsolute(new Vector3(-startPos.x, transform.position.y, startPos.y));
        if (!XRSettings.enabled)
        {
            characterController.height = unitSize / 2;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, unitSize / 2, Camera.main.transform.position.z),
                Quaternion.Euler(0, 0, 0));
        }
    }
}