using UnityEngine;

public class RollerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private TrackManager trackManager;

    [Header("Movement")]
    [SerializeField] private float laneWidth = 2.5f;
    [SerializeField] private float laneSwitchTime = 0.15f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private bool allowDoubleJump = false;

    private Rigidbody rb;
    private int currentLane = 1;
    private float targetX;
    private float laneVelocity;
    private float prevMoveX;
    private bool jumpRequested;
    private int groundContactCount;
    private bool doubleJumpUsed;

    public float ForwardSpeed => trackManager != null ? trackManager.WorldSpeed : 0f;
    public int CurrentLane => currentLane;
    public bool IsGrounded => groundContactCount > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        targetX = LaneToX(currentLane);
        Vector3 pos = rb.position;
        pos.x = targetX;
        rb.position = pos;
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.JumpEvent += OnJump;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.JumpEvent -= OnJump;
    }

    private void FixedUpdate()
    {
        ApplyLateralMovement();
        ApplyJump();
    }

    private void Update()
    {
        RollVisual();
    }

    private void ApplyLateralMovement()
    {
        float newX = Mathf.SmoothDamp(rb.position.x, targetX, ref laneVelocity, laneSwitchTime);
        Vector3 vel = rb.linearVelocity;
        vel.x = (newX - rb.position.x) / Time.fixedDeltaTime;
        rb.linearVelocity = vel;
    }

    private void RollVisual()
    {
        if (visualRoot == null) return;
        float speed = trackManager != null ? trackManager.WorldSpeed : 0f;
        float degrees = speed * Time.deltaTime * Mathf.Rad2Deg / sphereRadius;
        visualRoot.Rotate(Vector3.right, degrees, Space.Self);
    }

    private void ApplyJump()
    {
        if (!jumpRequested) return;
        jumpRequested = false;
        Vector3 vel = rb.linearVelocity;
        vel.y = jumpForce;
        rb.linearVelocity = vel;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && trackManager != null)
            trackManager.ResetSpeed();
        groundContactCount++;
        doubleJumpUsed = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        groundContactCount--;
    }

    private void OnJump(bool pressed)
    {
        if (!pressed) return;
        if (groundContactCount > 0)
            jumpRequested = true;
        else if (allowDoubleJump && !doubleJumpUsed)
        {
            jumpRequested = true;
            doubleJumpUsed = true;
        }
    }

    private void OnMove(Vector2 input)
    {
        float x = input.x;
        bool crossedRight = x >  0.5f && prevMoveX <=  0.5f;
        bool crossedLeft  = x < -0.5f && prevMoveX >= -0.5f;

        if (crossedRight)
            SetLane(currentLane + 1);
        else if (crossedLeft)
            SetLane(currentLane - 1);

        prevMoveX = x;
    }

    private void SetLane(int lane)
    {
        currentLane = Mathf.Clamp(lane, 0, 2);
        targetX = LaneToX(currentLane);
    }

    private float LaneToX(int lane) => (lane - 1) * laneWidth;
}
