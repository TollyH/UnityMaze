using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController character;

    private ControlMap inputActions;
    public float MovementSpeed;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject mapContainer;

    private bool runToggle = false;
    private bool crawlToggle = false;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        inputActions = levelManager.InputActions;
    }

    private void Update()
    {
        if (mapContainer.activeSelf
            || levelManager.MonsterManager.IsPlayerStruggling
            || levelManager.IsGameOver
            || levelManager.IsPaused)
        {
            return;
        }
        Vector2 inputValue = inputActions.PlayerMovement.PlayerMove.ReadValue<Vector2>();
        float moveMultiplier = MovementSpeed;
        if (runToggle || inputActions.PlayerMovement.RunModifier.IsPressed())
        {
            if (!levelManager.IsMulti || levelManager.MultiplayerManager.IsCoop)
            {
                moveMultiplier *= 2;
            }
        }
        if (crawlToggle || inputActions.PlayerMovement.CrawlModifier.IsPressed())
        {
            moveMultiplier *= 0.5f;
        }
        Vector3 movementVector = Camera.main.transform.TransformDirection(Time.deltaTime * moveMultiplier * new Vector3(inputValue.x, 0, inputValue.y));
        // If camera is pointing upwards (like in VR), distribute any upwards (y) movement between the x and z axis instead
        movementVector = new Vector3(movementVector.x + (movementVector.y / 2), 0, movementVector.z + (movementVector.y / 2));
        if (movementVector != Vector3.zero)
        {
            Vector3 oldPos = transform.position;
            _ = character.Move(movementVector);
            SendMessageUpwards("OnMove", (oldPos - transform.position).magnitude);
        }
    }

    private void OnRunToggle()
    {
        runToggle = !runToggle;
        crawlToggle = false;
    }

    private void OnCrawlToggle()
    {
        crawlToggle = !crawlToggle;
        runToggle = false;
    }
}
