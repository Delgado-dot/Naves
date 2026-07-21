using UnityEngine;

/// <summary>Generic laser projectile that moves forward, detects collisions with HealthComponent, applies damage, and spawns impact effects.</summary>
[RequireComponent(typeof(Collider))]
public class LaserProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 80f;
    [SerializeField] private float maxLifeTime = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private string damageType = "Laser";

    [Header("Collision")]
    [SerializeField] private LayerMask hitLayers = -1;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Impact Effects")]
    [SerializeField] private GameObject impactVFX;
    [SerializeField] private AudioClip impactSFX;
    [SerializeField] private float vfxLifetime = 2f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private bool alignVFXToNormal = true;

    private float lifeTimer;
    private Vector3 direction;

    private void Awake()
    {
        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.isTrigger = true;
    }

    private void OnEnable()
    {
        lifeTimer = 0f;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) == 0)
            return;

        var health = other.GetComponent<HealthComponent>();
        if (health != null)
        {
            var damageInfo = new DamageInfo(
                damage,
                transform.position,
                direction,
                gameObject,
                damageType
            );

            health.TakeDamage(damageInfo);
        }

        SpawnImpactEffect(transform.position, -direction);

        if (destroyOnHit)
        {
            DestroyProjectile();
        }
    }

    private void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (impactVFX != null)
        {
            Quaternion rotation = alignVFXToNormal && normal != Vector3.zero
                ? Quaternion.LookRotation(normal)
                : Quaternion.identity;

            var vfxInstance = Instantiate(impactVFX, position, rotation);
            Destroy(vfxInstance, vfxLifetime);
        }

        if (impactSFX != null)
        {
            AudioSource.PlayClipAtPoint(impactSFX, position, sfxVolume);
        }
    }

    /// <summary>Sets the direction and initializes the projectile.</summary>
    /// <param name="dir">Normalized direction vector.</param>
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>Sets the direction from a rotation.</summary>
    /// <param name="rotation">Rotation to face.</param>
    public void SetDirection(Quaternion rotation)
    {
        direction = rotation * Vector3.forward;
        transform.rotation = rotation;
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
        maxLifeTime = Mathf.Max(0.01f, maxLifeTime);
        damage = Mathf.Max(0f, damage);
    }
}