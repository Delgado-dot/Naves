using UnityEngine;

public class PlayerTilt : MonoBehaviour
{
    public Transform shipModel;

    public float maxTilt = 15f;
    public float tiltSpeed = 8f;

    private PlayerInputActions controls;
    private Vector2 moveInput;

    private void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Update()
    {
        float targetRoll = -moveInput.x * maxTilt;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetRoll);

        shipModel.localRotation = Quaternion.Lerp(
            shipModel.localRotation,
            targetRotation,
            tiltSpeed * Time.deltaTime);
    }
}