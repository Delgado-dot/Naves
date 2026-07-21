using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Fire Rate")]
    [SerializeField] private float fireCooldown = 0.15f;

    private InputAction attackAction;
    private bool useLeft = true;
    private float lastFireTime;

    private void Awake()
    {
        if (firePointLeft == null)
            firePointLeft = transform.Find("FirePointLeft");

        if (firePointRight == null)
            firePointRight = transform.Find("FirePointRight");

        attackAction = InputHelper.GetPlayerAction(inputActions, "Attack");
    }

    private void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.performed += OnAttackPerformed;
            attackAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.Disable();
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext _)
    {
        if (Time.time < lastFireTime + fireCooldown)
            return;

        Fire();

        lastFireTime = Time.time;
    }

    private void Fire()
    {
        if (projectilePrefab == null)
            return;

        Transform firePoint = useLeft ? firePointLeft : firePointRight;

        if (firePoint == null)
            return;

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        useLeft = !useLeft;
    }
}
