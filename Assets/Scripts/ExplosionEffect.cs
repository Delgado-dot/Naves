using UnityEngine;

/// <summary>Handles explosion effects when an object with HealthComponent dies.</summary>
[RequireComponent(typeof(HealthComponent))]
public class ExplosionEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelay = 0f;
    [SerializeField] private bool destroyAfterExplosionPrefab = true;
    [SerializeField] private float explosionLifetime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSFX;
    [SerializeField] private float sfxVolume = 1f;

    [Header("Behavior")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.5f;

    private HealthComponent health;
    private bool hasExploded;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += OnDeath;
        }
        hasExploded = false;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= OnDeath;
        }
    }

    private void OnDeath()
    {
        if (hasExploded)
            return;

        hasExploded = true;
        TriggerExplosion();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            if (destroyAfterExplosionPrefab)
            {
                Destroy(explosion, explosionLifetime);
            }
        }

        if (explosionSFX != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position, sfxVolume);
        }
    }
}