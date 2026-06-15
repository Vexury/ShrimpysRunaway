using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class RollerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private Renderer[] playerRenderers;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseForce = 1f;
    [SerializeField] private float impulseCooldown = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip rollClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip doubleJumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip laneSwapClip;

    [Header("Movement")]
    [SerializeField] private float laneWidth = 2.5f;
    [SerializeField] private float laneSwitchTime = 0.15f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float maxLeanAngle = 30f;
    [SerializeField] private bool allowDoubleJump = false;
    [SerializeField] private float jumpBufferTime = 0.15f;

    public bool HasDoubleJump { get; private set; }

    private Rigidbody rb;
    private int currentLane = 2;
    private float targetX;
    private float laneVelocity;
    private float prevMoveX;
    private Color originalColor;
    private float impulseLastFiredTime = float.MinValue;
    private float rollAngle;
    private float currentLeanY;
    private float leanTimer = -1f;
    private float leanSign;
    private float jumpBufferTimer;
    private int groundContactCount;
    private bool doubleJumpUsed;
    private bool wasGrounded;

    public static event Action OnObstacleHit;

    public float ForwardSpeed => trackManager != null ? trackManager.WorldSpeed : 0f;
    public int CurrentLane => currentLane;
    public bool IsGrounded => groundContactCount > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerRenderers != null && playerRenderers.Length > 0)
            originalColor = playerRenderers[0].material.color;
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
        Collectible.OnCollected += OnCollectibleCollected;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.JumpEvent -= OnJump;
        Collectible.OnCollected -= OnCollectibleCollected;
    }

    private void FixedUpdate()
    {
        ApplyLateralMovement();
        ApplyJump();
        rb.AddForce(Physics.gravity * (gravityScale - 1f) * rb.mass);
    }

    private void Update()
    {
        RollVisual();
        UpdateRollAudio();
    }

    private void UpdateRollAudio()
    {
        if (rollClip == null || AudioManager.Instance == null) return;
        bool grounded = IsGrounded;
        if (grounded && !wasGrounded)
        { 
            AudioManager.Instance.PlayLoopingSFX(rollClip, 0.3f);
            if (landClip != null) AudioManager.Instance.PlaySFX(landClip);
        }
        else if (!grounded && wasGrounded)
            AudioManager.Instance.StopLoopingSFX();
        wasGrounded = grounded;
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
        rollAngle += speed * Time.deltaTime * Mathf.Rad2Deg / sphereRadius;

        if (leanTimer >= 0f)
        {
            leanTimer += Time.deltaTime;
            float t = Mathf.Clamp01(leanTimer / laneSwitchTime);
            currentLeanY = Mathf.Sin(t * Mathf.PI) * maxLeanAngle * leanSign;
            if (t >= 1f) { leanTimer = -1f; currentLeanY = 0f; }
        }

        visualRoot.localRotation = Quaternion.Euler(0f, currentLeanY, 0f) * Quaternion.AngleAxis(rollAngle, Vector3.right);
    }

    private void ApplyJump()
    {
        if (jumpBufferTimer <= 0f) return;
        jumpBufferTimer -= Time.fixedDeltaTime;

        if (groundContactCount > 0)
        {
            jumpBufferTimer = 0f;
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;
            if (jumpClip != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(jumpClip);
        }
        else if ((allowDoubleJump || HasDoubleJump) && !doubleJumpUsed)
        {
            jumpBufferTimer = 0f;
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;
            doubleJumpUsed = true;
            HasDoubleJump = false;
            AudioClip clip = doubleJumpClip != null ? doubleJumpClip : jumpClip;
            if (clip != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(clip);
        }
    }

    private IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var r in playerRenderers) r.material.color = Color.red;
            if (impulseSource != null) impulseSource.GenerateImpulseWithForce(impulseForce / (i + 1));
            yield return new WaitForSeconds(blinkInterval);
            foreach (var r in playerRenderers) r.material.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Obstacle") || trackManager == null) return;

        trackManager.ResetSpeed();
        OnObstacleHit?.Invoke();
        if (Time.time - impulseLastFiredTime <= impulseCooldown) return;

        impulseLastFiredTime = Time.time;
        if (playerRenderers != null && playerRenderers.Length > 0)
            StartCoroutine(FlashRed());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Collectible>() != null) return;
        groundContactCount++;
        doubleJumpUsed = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Collectible>() != null) return;
        groundContactCount--;
    }

    private void OnCollectibleCollected(CollectibleType type, int count)
    {
        if (type == CollectibleType.Sandwich)
            HasDoubleJump = true;
    }

    private void OnJump(bool pressed)
    {
        if (!pressed) return;
        jumpBufferTimer = jumpBufferTime;
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
        int prev = currentLane;
        currentLane = Mathf.Clamp(lane, 0, 4);
        if (currentLane != prev)
        {
            leanSign = Mathf.Sign(currentLane - prev);
            leanTimer = 0f;
            if (laneSwapClip != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(laneSwapClip);
        }
        targetX = LaneToX(currentLane);
    }

    private float LaneToX(int lane) => (lane - 2) * laneWidth;
}
