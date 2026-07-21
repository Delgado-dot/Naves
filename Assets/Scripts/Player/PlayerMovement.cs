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
        currentSpeed = baseSpeed;
        targetSpeed = baseSpeed;
    }


    private void Update()
    {
        SpeedControl();
        MoveForward();
        Strafe();
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


        Vector3 lateral =
            transform.right * currentStrafe;


        transform.position += lateral * Time.deltaTime;
    }


    private void SpeedControl()
    {
        if (moveInput.y > 0)
        {
            targetSpeed = maxSpeed;
        }
        else if (moveInput.y < 0)
        {
            targetSpeed = minSpeed;
        }
        else
        {
            targetSpeed = baseSpeed;
        }


        if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }


        currentSpeed = Mathf.Clamp(
            currentSpeed,
            minSpeed,
            maxSpeed
        );
    }
}