using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 80f;
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
}