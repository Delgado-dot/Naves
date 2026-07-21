using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 5f;
    public float lifeTime = 3f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
