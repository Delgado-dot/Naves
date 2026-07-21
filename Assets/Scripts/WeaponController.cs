using UnityEngine;

/// <summary>Generic weapon controller for spaceships. Handles firing logic, cooldown, and projectile spawning.</summary>
public class WeaponController : MonoBehaviour
{
    [Header("Fire Points")]
    [SerializeField] private Transform[] firePoints;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 80f;

    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private string damageType = "Laser";
    [SerializeField] private bool useAlternatingFirePoints = true;

    [Header("Audio")]
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private float sfxVolume = 1f;

    private float nextFireTime;
    private int firePointIndex;

    public float FireRate => fireRate;
    public float Damage => damage;
    public GameObject ProjectilePrefab => projectilePrefab;
    public Transform[] FirePoints => firePoints;

    private void OnValidate()
    {
        fireRate = Mathf.Max(0.01f, fireRate);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        damage = Mathf.Max(0f, damage);

        if (firePoints != null)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                if (firePoints[i] == null)
                {
                    Debug.LogWarning($"WeaponController on {name}: FirePoint at index {i} is null.", this);
                }
            }
        }
    }

    /// <summary>Attempts to fire if cooldown allows.</summary>
    /// <returns>True if a shot was fired.</returns>
    public bool TryFire()
    {
        if (Time.time < nextFireTime)
            return false;

        if (projectilePrefab == null)
        {
            Debug.LogWarning($"WeaponController on {name}: No projectile prefab assigned.", this);
            return false;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning($"WeaponController on {name}: No fire points assigned.", this);
            return false;
        }

        Fire();
        nextFireTime = Time.time + fireRate;
        return true;
    }

    /// <summary>Fires immediately without cooldown check.</summary>
    public void ForceFire()
    {
        if (projectilePrefab == null || firePoints == null || firePoints.Length == 0)
            return;

        Fire();
    }

    private void Fire()
    {
        Transform firePoint = GetNextFirePoint();
        if (firePoint == null)
            return;

        var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        var laser = projectile.GetComponent<LaserProjectile>();
        if (laser != null)
        {
            laser.SetDirection(firePoint.forward);
        }
        else
        {
            var rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * projectileSpeed;
            }
            else
            {
                var simpleProjectile = projectile.GetComponent<Projectile>();
                if (simpleProjectile != null)
                {
                    simpleProjectile.SetDirection(firePoint.forward);
                }
            }
        }

        if (fireSFX != null)
        {
            AudioSource.PlayClipAtPoint(fireSFX, firePoint.position, sfxVolume);
        }
    }

    private Transform GetNextFirePoint()
    {
        if (firePoints.Length == 0)
            return null;

        if (useAlternatingFirePoints && firePoints.Length > 1)
        {
            Transform fp = firePoints[firePointIndex];
            firePointIndex = (firePointIndex + 1) % firePoints.Length;
            return fp;
        }

        return firePoints[0];
    }

    /// <summary>Sets fire rate at runtime.</summary>
    public void SetFireRate(float rate)
    {
        fireRate = Mathf.Max(0.01f, rate);
    }

    /// <summary>Sets damage at runtime.</summary>
    public void SetDamage(float dmg)
    {
        damage = Mathf.Max(0f, dmg);
    }

    /// <summary>Sets projectile prefab at runtime.</summary>
    public void SetProjectilePrefab(GameObject prefab)
    {
        projectilePrefab = prefab;
    }

    /// <summary>Adds a fire point at runtime.</summary>
    public void AddFirePoint(Transform point)
    {
        if (point == null) return;

        var list = new System.Collections.Generic.List<Transform>(firePoints ?? new Transform[0]);
        if (!list.Contains(point))
        {
            list.Add(point);
            firePoints = list.ToArray();
        }
    }
}