using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 80f;
    public float damage = 1f;
    public float lifeTime = 3f;

    private Vector3 direction;
    private bool targetsPlayer;
    private Collider projectileCollider;

    private void Awake()
    {
        projectileCollider = GetComponent<Collider>();
    }


    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        targetsPlayer = false;
    }

    public void ConfigureEnemyShot(Vector3 dir, float shotSpeed, float shotDamage, Collider[] ownerColliders)
    {
        direction = dir.normalized;
        speed = shotSpeed;
        damage = shotDamage;
        targetsPlayer = true;

        if (projectileCollider == null || ownerColliders == null)
            return;

        foreach (Collider ownerCollider in ownerColliders)
        {
            if (ownerCollider != null)
                Physics.IgnoreCollision(projectileCollider, ownerCollider);
        }
    }


    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }


    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHit(collision.collider);
    }

    private void TryHit(Collider other)
    {
        if (targetsPlayer)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null || playerHealth.IsDead)
                return;

            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Enemy enemyComponent = other.GetComponentInParent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
