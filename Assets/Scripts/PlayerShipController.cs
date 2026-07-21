using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private Vector2 moveInput;
    private bool isMoving;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned.", this);
            return;
        }

        var playerMap = inputActions.FindActionMap("Player", false);

        if (playerMap == null)
        {
            Debug.LogError("Player action map not found.", this);
            return;
        }

        moveAction = playerMap.FindAction("Move", false);

        if (moveAction == null)
        {
            Debug.LogError("Move action not found in Player map.", this);
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
            moveInput = Vector2.zero;
            isMoving = false;
        }
    }

    private void Update()
    {
        if (moveAction == null)
            return;

        moveInput = moveAction.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0f;

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (!isMoving)
            return;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
