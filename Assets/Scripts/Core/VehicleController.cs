using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float maxForwardSpeed = 12f;
    [SerializeField] private float maxReverseSpeed = 5f;
    [SerializeField] private float boostSpeed = 20f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float brakeForce = 15f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Steering")]
    [SerializeField] private float steerSpeed = 90f;
    [SerializeField] private float minSteerSpeedAtMaxSpeed = 30f;
    [SerializeField] private float minSpeedToSteer = 0.5f;

    [Header("Features")]
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private bool useReverseGear = true;
    [SerializeField] private bool useSpeedDependentSteering = true;

    public float CurrentSpeed => currentSpeed;
    public float NormalizedSpeed { get; private set; }
    public bool IsGrounded => characterController.isGrounded;
    public bool IsBoosting => isBoosting;

    private Vector2 moveInput;
    private bool isBoosting;
    private float currentSpeed;
    private float verticalVelocity;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        inputReader.EnableGameplayInput();
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.SprintEvent += OnBoost;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.SprintEvent -= OnBoost;
    }

    private void Update()
    {
        HandleSteering();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float throttle = moveInput.y;
        float topForwardSpeed = isBoosting ? boostSpeed : maxForwardSpeed;

        float targetSpeed;
        if (throttle > 0f)
            targetSpeed = throttle * topForwardSpeed;
        else if (throttle < 0f)
            targetSpeed = useReverseGear ? throttle * maxReverseSpeed : 0f;
        else
            targetSpeed = 0f;

        if (useAcceleration)
        {
            bool isBraking = (throttle < 0f && currentSpeed > 0f) || (throttle > 0f && currentSpeed < 0f);
            float rate = isBraking ? brakeForce : Mathf.Abs(throttle) < 0.01f ? deceleration : acceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        float normMax = isBoosting ? boostSpeed : maxForwardSpeed;
        NormalizedSpeed = Mathf.Clamp01(Mathf.InverseLerp(0f, normMax, Mathf.Abs(currentSpeed)));

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = transform.forward * currentSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    private void HandleSteering()
    {
        if (Mathf.Abs(currentSpeed) < minSpeedToSteer) return;

        float effectiveSteerSpeed;
        if (useSpeedDependentSteering)
        {
            float topSpeed = isBoosting ? boostSpeed : maxForwardSpeed;
            float speedFraction = Mathf.Abs(currentSpeed) / topSpeed;
            effectiveSteerSpeed = Mathf.Lerp(steerSpeed, minSteerSpeedAtMaxSpeed, speedFraction);
        }
        else
        {
            effectiveSteerSpeed = steerSpeed;
        }

        float steerDirection = (useReverseGear && currentSpeed < 0f) ? -1f : 1f;
        transform.Rotate(0f, moveInput.x * effectiveSteerSpeed * steerDirection * Time.deltaTime, 0f);
    }

    private void OnMove(Vector2 input) => moveInput = input;
    private void OnBoost(bool isPressed) => isBoosting = isPressed;
}
