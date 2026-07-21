using UnityEngine;

/// <summary>
/// Enemigo espacial: se mueve en 3D directo hacia el jugador.
/// IMPORTANTE: asignar tag "Enemy" al prefab y agregar un Collider como Trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpaceEnemy : Enemy
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Dano al colisionar")]
    [SerializeField] private float collisionDamage = 10f;

    [Header("Opcional: orbita")]
    [SerializeField] private bool useOrbit = false;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 2f;

    private float orbitAngle;
    private float spawnGracePeriod = 0.5f;
    private float spawnTimer;

    public override void Initialize(Transform chaseTarget)
    {
        base.Initialize(chaseTarget);
        spawnTimer = spawnGracePeriod;
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

        if (useOrbit && distance < orbitRadius * 2f)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle), 0f) * orbitRadius;
            Vector3 targetPos = target.position + orbitOffset;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnTimer > 0f) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(collisionDamage);
                Die();
            }
        }
    }
}
