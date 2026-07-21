using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Transform aimTarget;

    public float fireRate = 0.2f;

    private PlayerInputActions controls;
    private float nextFireTime;


    private void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Fire.performed += ctx =>
        {
            Shoot();
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


    private void Shoot()
    {
        if (Time.time < nextFireTime)
            return;


        nextFireTime = Time.time + fireRate;


        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );


        Vector3 direction =
            (aimTarget.position - firePoint.position).normalized;

        Debug.Log("Direccion disparo: " + direction);


        bullet.GetComponent<Projectile>()
            .SetDirection(direction);
    }
}
