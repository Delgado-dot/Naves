using UnityEngine;

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
}
