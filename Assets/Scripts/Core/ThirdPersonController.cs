using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTarget; // Pivot point the camera orbits around

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float normalJumpHeight = 2f;
    [SerializeField] private float featherJumpHeight = 4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f; // How fast the character turns to face movement dir

    [Header("Audio")]
    [SerializeField] private PlayerMovementAudio movementAudio;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 45f;
    [SerializeField] private float minLookAngle = -20f; // Prevent camera clipping into ground

    public bool featherMode = false;

    public float NormalizedSpeed { get; private set; }
    public bool IsGrounded => characterController.isGrounded;
    public bool JumpedThisFrame { get; private set; }

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private bool isCursorUnlocked = false;
    private bool hasReceivedFirstInput;
    private float verticalVelocity;
    private float cameraYaw;   // Horizontal orbit angle
    private float cameraPitch; // Vertical orbit angle

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;

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
        cameraYaw = transform.eulerAngles.y; // Initialize yaw to character's facing direction
    }

    private void Update()
    {
        JumpedThisFrame = false;
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        if (isCursorUnlocked) return;

        bool isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Movement is relative to the camera's horizontal facing direction
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up).normalized;
        Vector3 movement = (camForward * moveInput.y + camRight * moveInput.x) * currentSpeed;

        // Rotate character to face movement direction
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);

        float horizontalSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
        NormalizedSpeed = Mathf.InverseLerp(0f, sprintSpeed, horizontalSpeed);
    }

    private void HandleLook()
    {
        if (isCursorUnlocked) return;

        // Orbit the camera around the cameraTarget pivot
        cameraYaw += lookInput.x * lookSensitivity;
        cameraPitch -= lookInput.y * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        cameraTarget.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    private void OnMove(Vector2 input) => moveInput = input;

    private void OnLook(Vector2 input)
    {
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
            JumpedThisFrame = true;
            movementAudio.PlayJumpSound();
        }
    }

    private void OnSprint(bool isPressed) => isSprinting = isPressed;

    private void OnUnlockCursor(bool isPressed)
    {
        isCursorUnlocked = isPressed;
        Cursor.lockState = isPressed ? CursorLockMode.None : CursorLockMode.Locked;
    }
}