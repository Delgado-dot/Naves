using UnityEngine;
using UnityEngine.InputSystem;

public class MouseAim : MonoBehaviour
{
    public RectTransform crosshair;

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
        crosshair.position = mousePosition;
    }
}