using UnityEngine;

public class ThirdPersonFreeLookController : PlayerController
{
    [Header("Look")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 45f;
    [SerializeField] private float minLookAngle = -20f;

    [Header("Feather Jump")]
    [SerializeField] private float featherJumpHeight = 4f;
    public bool featherMode = false;

    [Header("Audio")]
    [SerializeField] private PlayerMovementAudio movementAudio;

    private Vector2 lookInput;
    private bool isCursorUnlocked;
    private bool hasReceivedFirstInput;
    private float cameraYaw;
    private float cameraPitch;

    protected override void Start()
    {
        base.Start();
        Cursor.lockState = CursorLockMode.Locked;
        cameraYaw = transform.eulerAngles.y;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputReader.LookEvent         += OnLook;
        inputReader.UnlockCursorEvent += OnUnlockCursor;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputReader.LookEvent         -= OnLook;
        inputReader.UnlockCursorEvent -= OnUnlockCursor;
    }

    protected override void Update()
    {
        base.Update();
        HandleLook();
    }

    protected override Vector3 ComputeHorizontalMove()
    {
        if (isCursorUnlocked) return Vector3.zero;

        Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(playerCamera.transform.right,   Vector3.up).normalized;
        return (camForward * moveInput.y + camRight * moveInput.x) * CurrentSpeed;
    }

    protected override void OnJump(bool isPressed)
    {
        if (!isPressed || !characterController.isGrounded) return;

        float height = featherMode ? featherJumpHeight : jumpHeight;
        verticalVelocity = Mathf.Sqrt(height * -2f * gravity);
        JumpedThisFrame = true;
        if (movementAudio != null) movementAudio.PlayJumpSound();
    }

    private void HandleLook()
    {
        if (isCursorUnlocked) return;

        cameraYaw   += lookInput.x * lookSensitivity;
        cameraPitch -= lookInput.y * lookSensitivity;
        cameraPitch  = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        cameraTarget.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    private void OnLook(Vector2 input)
    {
        if (!hasReceivedFirstInput) { hasReceivedFirstInput = true; return; }
        lookInput = input;
    }

    private void OnUnlockCursor(bool isPressed)
    {
        isCursorUnlocked = isPressed;
        Cursor.lockState = isPressed ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
