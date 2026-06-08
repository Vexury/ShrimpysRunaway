using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected InputReader inputReader;
    [SerializeField] protected CharacterController characterController;
    [SerializeField] protected Camera playerCamera;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float sprintSpeed = 8f;
    [SerializeField] protected float crouchSpeed = 2.5f;
    [SerializeField] protected float rotationSpeed = 10f;
    [SerializeField] protected float gravity = -9.81f;
    [SerializeField] protected float jumpHeight = 2f;
    [SerializeField] protected float crouchHeight = 1f;

    public float NormalizedSpeed { get; private set; }
    public bool JumpedThisFrame { get; protected set; }
    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsGrounded => characterController.isGrounded;

    protected Vector2 moveInput;
    protected float verticalVelocity;
    protected float standingHeight;

    protected virtual void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        standingHeight = characterController.height;
        inputReader.EnableGameplayInput();
    }

    protected virtual void OnEnable()
    {
        inputReader.MoveEvent   += OnMove;
        inputReader.JumpEvent   += OnJump;
        inputReader.SprintEvent += OnSprint;
        inputReader.CrouchEvent += OnCrouch;
    }

    protected virtual void OnDisable()
    {
        inputReader.MoveEvent   -= OnMove;
        inputReader.JumpEvent   -= OnJump;
        inputReader.SprintEvent -= OnSprint;
        inputReader.CrouchEvent -= OnCrouch;
    }

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    protected virtual void Update()
    {
        JumpedThisFrame = false;

        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        Vector3 move = ComputeHorizontalMove();

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);

        float horizontalSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
        NormalizedSpeed = Mathf.InverseLerp(0f, sprintSpeed, horizontalSpeed);
    }

    protected abstract Vector3 ComputeHorizontalMove();

    protected float CurrentSpeed => IsCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : moveSpeed;

    private void OnMove(Vector2 input) => moveInput = input;

    private void OnSprint(bool isPressed) => IsSprinting = isPressed;

    private void OnCrouch(bool isPressed)
    {
        IsCrouching = isPressed;
        float targetHeight = isPressed ? crouchHeight : standingHeight;
        characterController.height = targetHeight;
        characterController.center = new Vector3(0f, targetHeight / 2f, 0f);
    }

    protected virtual void OnJump(bool isPressed)
    {
        if (isPressed && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            JumpedThisFrame = true;
        }
    }
}
