using System;
using UnityEngine;

/// <summary>
/// Clase base para todos los enemigos. Maneja vida, dano,
/// muerte y notificacion al spawner para liberar cupo.
/// </summary>
[DisallowMultipleComponent]
public abstract class Enemy : MonoBehaviour
{
    [Header("Configuracion base")]
    [SerializeField] protected float maxHealth = 30f;
    [SerializeField] protected float scoreValue = 100f;

    protected float currentHealth;
    protected Transform target;

    private bool hasNotifiedDeath;

    /// <summary>Se dispara cuando el enemigo muere. El spawner se suscribe.</summary>
    public event Action<Enemy> Died;

    /// <summary>Puntos que otorga al morir.</summary>
    public float ScoreValue => scoreValue;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>Llamado por el spawner despues de instanciar.</summary>
    public virtual void Initialize(Transform chaseTarget)
    {
        target = chaseTarget;
        currentHealth = maxHealth;
        hasNotifiedDeath = false;
    }

    public virtual void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (ScoreManager.Instance != null && scoreValue > 0f)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        NotifyDeath();
        Destroy(gameObject);
    }

    private void NotifyDeath()
    {
        if (hasNotifiedDeath) return;
        hasNotifiedDeath = true;
        Died?.Invoke(this);
    }

    protected virtual void OnDestroy()
    {
        NotifyDeath();
    }
}
