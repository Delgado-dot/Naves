using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 80f;
    public float damage = 1f;
    public float lifeTime = 3f;

    private Vector3 direction;


    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
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
        if (!other.CompareTag("Enemy"))
            return;

        Enemy enemyComponent = other.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}