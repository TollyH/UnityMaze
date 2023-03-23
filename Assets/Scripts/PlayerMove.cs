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
        Vector3 movementVector = Time.deltaTime * moveMultiplier * new Vector3(inputValue.x, 0, inputValue.y);
        _ = character.Move(transform.TransformDirection(movementVector));
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
