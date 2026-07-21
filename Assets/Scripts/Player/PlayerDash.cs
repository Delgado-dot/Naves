
using UnityEngine;
using System.Collections;

using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash 360")]
    public float dashDistance = 30f;
    public float dashDuration = 0.25f;
    public float rotationSpeed = 1440f;
    public float dashCooldown = 1f;


    private PlayerInputActions controls;
    public Transform shipModel;
    private Vector2 moveInput;
    private bool canDash = true;
    private bool boostPressed;
    private Quaternion originalRotation;

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

        controls.Player.Dash.performed += _ =>
        {
            TryDash();
        };
    }
    private void Start()
    {
        originalRotation = shipModel.localRotation;
    }

    private void OnEnable()
    {
        controls.Enable();
    }


    private void OnDisable()
    {
        controls.Disable();
    }
    private void TryDash()
    {
        if (!canDash)
            return;


        if (Mathf.Abs(moveInput.x) < 0.5f)
            return;


        StartCoroutine(PerformDash());
    }
    private IEnumerator PerformDash()
    {
        canDash = false;


        float direction = Mathf.Sign(moveInput.x);

        float elapsed = 0f;


        float dashSpeed = dashDistance / dashDuration;


        while (elapsed < dashDuration)
        {
            // Salto lateral largo
            float smoothDash = Mathf.Lerp(
                dashSpeed,
                0,
                elapsed / dashDuration
            );

            transform.position +=
                transform.right *
                direction *
                smoothDash *
                Time.deltaTime;


            // Giro 360 grados mientras se mueve
            shipModel.Rotate(
                Vector3.forward,
                -direction * rotationSpeed * Time.deltaTime,
                Space.Self
            );


            elapsed += Time.deltaTime;


            yield return null;
        }

        shipModel.localRotation = originalRotation;
        yield return new WaitForSeconds(dashCooldown);


        canDash = true;
    }
}