using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    public float baseSpeed = 20f;
    public float maxSpeed = 35f;
    public float minSpeed = 10f;

    public float acceleration = 15f;
    public float deceleration = 10f;

    [Header("Movimiento Lateral")]
    public float strafeSpeed = 12f;
    public float strafeSmooth = 8f;

    [Header("Boost")]
    public float boostSpeed = 90f;
    public float boostDuration = 4f;

    public float maxBoostEnergy = 100f;
    public float boostConsumption = 25f;
    public float rechargeSpeed = 15f;

    [Header("Cámara")]
    public CinemachineCamera cinemachineCamera;

    public float normalFOV = 60f;
    public float boostFOV = 75f;
    public float fovSpeed = 8f;

    private bool isBoostActive;
    private float boostEnergy;
    private bool boosting;
    private bool wasBoosting;
    private float boostTimer;
    public ParticleSystem leftEngine;
    public ParticleSystem rightEngine;
    private float currentSpeed;
    private float targetSpeed;

    private float currentStrafe;

    private PlayerInputActions controls;
    private Vector2 moveInput;
    

    private void Awake()
    {
        
        controls = new PlayerInputActions();

        controls.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
        };

        controls.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
        };
        controls.Player.Boost.performed += _ => boosting = true;
        controls.Player.Boost.canceled += _ => boosting = false;
    }


    private void OnEnable()
    {
        controls.Enable();
    }


    private void OnDisable()
    {
        controls.Disable();
    }


    private void Start()
    {
        boostEnergy = maxBoostEnergy;
        currentSpeed = baseSpeed;
        targetSpeed = baseSpeed;
    }


    private void Update()
    {
        HandleBoost();
        SpeedControl();
        MoveForward();
        Strafe();
        UpdateCameraFOV();
    }


    private void MoveForward()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }


    private void Strafe()
    {
        float targetStrafe = moveInput.x * strafeSpeed;

        currentStrafe = Mathf.Lerp(
            currentStrafe,
            targetStrafe,
            strafeSmooth * Time.deltaTime
        );

        Vector3 lateral = transform.right * currentStrafe;

        transform.position += lateral * Time.deltaTime;
    }


    private void SpeedControl()
    {
        if (moveInput.y > 0)
        {
            targetSpeed = isBoostActive ? boostSpeed : maxSpeed;
        }
        else if (moveInput.y < 0)
        {
            targetSpeed = minSpeed;
        }
        else
        {
            targetSpeed = baseSpeed;
        }


        float accel = acceleration;

        if (currentSpeed < targetSpeed)
        {
            currentSpeed += accel * Time.deltaTime;
        }
    
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }

        float maxAllowedSpeed = isBoostActive ? boostSpeed : maxSpeed;

        currentSpeed = Mathf.Clamp(
            currentSpeed,
            minSpeed,
            maxAllowedSpeed
        );
    }
    private void HandleBoost()
    {
        var leftEmission = leftEngine.emission;
        var rightEmission = rightEngine.emission;


        // Detecta cuando se presiona Shift por primera vez
        if (boosting && !wasBoosting && boostEnergy > 0)
        {
            isBoostActive = true;

            currentSpeed = boostSpeed;

            boostTimer = boostDuration;

            boostEnergy -= boostConsumption * boostDuration;

            boostEnergy = Mathf.Max(boostEnergy, 0f);
        }


        wasBoosting = boosting;


        if (isBoostActive)
        {
            boostTimer -= Time.deltaTime;


            leftEmission.rateOverTime = 200f;
            rightEmission.rateOverTime = 200f;


            if (boostTimer <= 0)
            {
                isBoostActive = false;
            }
        }
        else
        {
            boostEnergy += rechargeSpeed * Time.deltaTime;
            boostEnergy = Mathf.Min(boostEnergy, maxBoostEnergy);


            leftEmission.rateOverTime = 0f;
            rightEmission.rateOverTime = 0f;
        }
    }
    private void UpdateCameraFOV()
    {
        if (cinemachineCamera == null)
            return;

        float targetFOV = isBoostActive
            ? boostFOV
            : normalFOV;

        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
            cinemachineCamera.Lens.FieldOfView,
            targetFOV,
            fovSpeed * Time.deltaTime
        );
    }
}