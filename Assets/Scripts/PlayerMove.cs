using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController character;

    private ControlMap inputActions;
    public float MovementSpeed;

    private bool runToggle = false;
    private bool crawlToggle = false;

    private void Awake()
    {
        inputActions = new ControlMap();
    }

    private void Start()
    {
        character = GetComponent<CharacterController>();
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
        Vector2 inputValue = inputActions.PlayerMovement.PlayerMove.ReadValue<Vector2>();
        float moveMultiplier = MovementSpeed;
        if (runToggle || inputActions.PlayerMovement.RunModifier.IsPressed())
        {
            moveMultiplier *= 2;
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
            _ = character.Move(movementVector);
            SendMessageUpwards("OnMove");
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
