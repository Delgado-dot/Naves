using System.Collections.Generic;
using UnityEngine;

/// <summary>Simple object pool for laser projectiles to avoid frequent instantiation/destruction.</summary>
public class LaserProjectilePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private LaserProjectile prefab;
    [SerializeField] private int initialSize = 20;
    [SerializeField] private int maxSize = 100;
    [SerializeField] private bool autoExpand = true;

    private readonly Queue<LaserProjectile> available = new Queue<LaserProjectile>();
    private readonly HashSet<LaserProjectile> active = new HashSet<LaserProjectile>();

    public static LaserProjectilePool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (prefab == null)
        {
            Debug.LogError("LaserProjectilePool: Prefab not assigned.", this);
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            CreateProjectile();
        }
    }

    private LaserProjectile CreateProjectile()
    {
        var projectile = Instantiate(prefab, transform);
        projectile.gameObject.SetActive(false);
        available.Enqueue(projectile);
        return projectile;
    }

    /// <summary>Gets a projectile from the pool, initializing it at position with rotation.</summary>
    public LaserProjectile Get(Vector3 position, Quaternion rotation)
    {
        LaserProjectile projectile;

        if (available.Count > 0)
        {
            projectile = available.Dequeue();
        }
        else if (autoExpand && active.Count < maxSize)
        {
            projectile = CreateProjectile();
            available.Dequeue();
        }
        else
        {
            return null;
        }

        projectile.transform.SetPositionAndRotation(position, rotation);
        projectile.gameObject.SetActive(true);
        active.Add(projectile);

        return projectile;
    }

    /// <summary>Returns a projectile to the pool.</summary>
    public void Return(LaserProjectile projectile)
    {
        if (projectile == null || !active.Contains(projectile))
            return;

        active.Remove(projectile);
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(transform);
        available.Enqueue(projectile);
    }

    /// <summary>Returns all active projectiles to the pool.</summary>
    public void ReturnAll()
    {
        var toReturn = new List<LaserProjectile>(active);
        foreach (var p in toReturn)
        {
            Return(p);
        }
    }

    public int AvailableCount => available.Count;
    public int ActiveCount => active.Count;
}