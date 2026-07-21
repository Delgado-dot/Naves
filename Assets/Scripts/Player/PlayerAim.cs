using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Referencias")]
    public Transform aimTarget;

    [Header("Ajustes")]
    public float aimDistance = 100f;

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
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        aimTarget.position = ray.origin + ray.direction * aimDistance;
    }
}