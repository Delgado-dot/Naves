using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Referencias")]
    public Transform aimTarget;

    [Header("Ajustes")]
    public float aimDistance = 100f;
    [Min(1f)] public float turnSpeed = 360f;

    private PlayerInputActions controls;
    private Vector2 mousePosition;


    private void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Look.performed += ctx =>
        {
            mousePosition = ctx.ReadValue<Vector2>();
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


    private void Update()
    {
        Aim();
    }


    private void Aim()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null || aimTarget == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Vector3 aimDirection = ray.direction.normalized;

        if (aimDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime);
        }

        aimTarget.position = ray.origin + aimDirection * aimDistance;
    }
}
