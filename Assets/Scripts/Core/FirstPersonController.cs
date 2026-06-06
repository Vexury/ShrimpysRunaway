using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTarget;


    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float normalJumpHeight = 2f;
    [SerializeField] private float featherJumpHeight = 4f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Audio")]
    [SerializeField] private PlayerMovementAudio movementAudio;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 45f;
    [SerializeField] private float minLookAngle = -45f;

    public bool featherMode = false;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private bool isCursorUnlocked = false;
    private bool hasReceivedFirstInput;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        // Auto-find components if not assigned
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        inputReader.EnableGameplayInput();
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.LookEvent += OnLook;
        inputReader.JumpEvent += OnJump;
        inputReader.SprintEvent += OnSprint;
        inputReader.UnlockCursorEvent += OnUnlockCursor;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.LookEvent -= OnLook;
        inputReader.JumpEvent -= OnJump;
        inputReader.SprintEvent -= OnSprint;
        inputReader.UnlockCursorEvent -= OnUnlockCursor;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        if (PauseManager.Instance.IsPaused()) return;
        if (isCursorUnlocked) return;

        // Ground check
        bool isGrounded = characterController.isGrounded;

        // Reset vertical velocity when grounded
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Stick on ground
        }

        // Calculate movement direction
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement *= currentSpeed;

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        // Move the character
        characterController.Move(movement * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (PauseManager.Instance.IsPaused()) return;
        if (isCursorUnlocked) return;

        // Rotate player body (Y-axis)
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);
        // Rotate camera (X-axis)
        cameraPitch -= lookInput.y * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);
        cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void OnLook(Vector2 input)
    {
        // Ignore the very first look input to prevent initial camera snap
        if (!hasReceivedFirstInput)
        {
            hasReceivedFirstInput = true;
            return;
        }

        lookInput = input;
    }

    private void OnJump(bool isPressed)
    {
        if (isPressed && characterController.isGrounded)
        {
            float jumpHeight = featherMode ? featherJumpHeight : normalJumpHeight;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            movementAudio.PlayJumpSound();
        }
    }

    private void OnSprint(bool isPressed)
    {
        isSprinting = isPressed;
    }

    private void OnUnlockCursor(bool isPressed)
    {
        isCursorUnlocked = isPressed;

        if (isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}