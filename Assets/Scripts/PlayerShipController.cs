using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Rotation")]
    [SerializeField] private float yawSensitivity = 2f;
    [SerializeField] private float pitchSensitivity = 1.5f;
    [SerializeField] private float maxPitchAngle = 85f;
    [SerializeField] private float rollIntensity = 25f;
    [SerializeField] private float rollSmoothTime = 0.2f;

    private InputAction moveAction;
    private InputAction lookAction;
    private Vector2 moveInput;
    private bool isMoving;
    private float totalYaw;
    private float totalPitch;
    private float currentRoll;
    private float rollVelocity;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned.", this);
            return;
        }

        if (inputActions.FindActionMap("Player", false) == null)
        {
            Debug.LogError("Player action map not found.", this);
            return;
        }

        moveAction = InputHelper.GetPlayerAction(inputActions, "Move");
        lookAction = InputHelper.GetPlayerAction(inputActions, "Look");
    }

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        totalYaw = euler.y;
        float pitch = euler.x;
        totalPitch = pitch > 180f ? pitch - 360f : pitch;
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.Enable();

        if (lookAction != null)
            lookAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
            moveInput = Vector2.zero;
            isMoving = false;
        }

        if (lookAction != null)
        {
            lookAction.Disable();
            totalYaw = 0f;
            totalPitch = 0f;
            currentRoll = 0f;
            rollVelocity = 0f;
        }
    }

    private void Update()
    {
        if (moveAction == null)
            return;

        moveInput = moveAction.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0f;

        ApplyMovement();
        ApplyRotation();
    }

    private void ApplyMovement()
    {
        if (!isMoving)
            return;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void ApplyRotation()
    {
        if (lookAction == null)
            return;

        Vector2 look = lookAction.ReadValue<Vector2>();

        totalYaw += look.x * yawSensitivity;
        totalPitch -= look.y * pitchSensitivity;
        totalPitch = Mathf.Clamp(totalPitch, -maxPitchAngle, maxPitchAngle);

        float targetRoll = Mathf.Clamp(-look.x * rollIntensity, -rollIntensity, rollIntensity);
        currentRoll = Mathf.SmoothDamp(currentRoll, targetRoll, ref rollVelocity, rollSmoothTime);

        Quaternion yawRot = Quaternion.Euler(0f, totalYaw, 0f);
        Quaternion pitchRot = Quaternion.Euler(totalPitch, 0f, 0f);
        Quaternion rollRot = Quaternion.Euler(0f, 0f, currentRoll);

        transform.rotation = yawRot * pitchRot * rollRot;
    }
}
