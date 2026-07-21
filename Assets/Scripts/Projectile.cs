using UnityEngine;

/// <summary>Projectile that moves forward and detects hits on Enemy-tagged objects.</summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        HandleEnemyHit(other.gameObject);

        Destroy(gameObject);
    }

    protected virtual void HandleEnemyHit(GameObject enemy)
    {
    }
}