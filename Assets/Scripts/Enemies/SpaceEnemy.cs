using UnityEngine;

/// <summary>
/// Enemigo espacial: se mueve en 3D directo hacia el jugador.
/// IMPORTANTE: asignar tag "Enemy" al prefab y agregar un Collider como Trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpaceEnemy : Enemy
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 22f;

    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Min(0.1f)] private float fireInterval = 1.4f;
    [SerializeField, Min(1f)] private float fireRange = 70f;
    [SerializeField, Min(0f)] private float projectileDamage = 8f;
    [SerializeField, Min(1f)] private float projectileSpeed = 55f;
    [SerializeField, Min(0f)] private float muzzleOffset = 2.6f;

    [Header("Dano al colisionar")]
    [SerializeField] private float collisionDamage = 10f;

    [Header("Opcional: orbita")]
    [SerializeField] private bool useOrbit = false;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 2f;

    private float orbitAngle;
    private float spawnGracePeriod = 0.5f;
    private float spawnTimer;
    private float nextFireTime;
    private Collider[] ownColliders;

    protected override void Awake()
    {
        base.Awake();
        ownColliders = GetComponentsInChildren<Collider>();
    }

    public override void Initialize(Transform chaseTarget)
    {
        base.Initialize(chaseTarget);
        spawnTimer = spawnGracePeriod;
        nextFireTime = Time.time + Random.Range(0.25f, fireInterval);
    }

    private void Update()
    {
        if (spawnTimer > 0f)
        {
            spawnTimer -= Time.deltaTime;
        }

        if (target == null) return;

        Vector3 directionToTarget = target.position - transform.position;
        float distance = directionToTarget.magnitude;

        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if (useOrbit && distance <= stoppingDistance)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle), 0f) * orbitRadius;
            Vector3 targetPos = target.position + orbitOffset;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
        else if (distance > stoppingDistance)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        if (spawnTimer <= 0f && distance <= fireRange)
            TryShoot(directionToTarget);
    }

    private void TryShoot(Vector3 directionToTarget)
    {
        if (projectilePrefab == null || Time.time < nextFireTime || directionToTarget.sqrMagnitude < 0.01f)
            return;

        nextFireTime = Time.time + fireInterval;
        Vector3 direction = directionToTarget.normalized;
        Vector3 spawnPosition = transform.position + direction * muzzleOffset;
        GameObject shot = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        Projectile projectile = shot.GetComponent<Projectile>();

        if (projectile != null)
            projectile.ConfigureEnemyShot(direction, projectileSpeed, projectileDamage, ownColliders);
        else
            Destroy(shot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnTimer > 0f) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(collisionDamage);
                Die();
            }
        }
    }
}
