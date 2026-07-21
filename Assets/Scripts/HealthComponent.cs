using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Generic health component that can be added to any GameObject to handle damage and death.</summary>
public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Events")]
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent<float> onHealthChanged;
    [SerializeField] private UnityEvent<DamageInfo> onDamaged;

    private bool isDead;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float NormalizedHealth => maxHealth > 0f ? currentHealth / maxHealth : 0f;
    public bool IsDead => isDead;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    public event Action<DamageInfo> OnDamaged;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        isDead = false;
    }

    private void Start()
    {
        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>Applies damage to this object.</summary>
    /// <param name="damageInfo">Information about the damage being applied.</param>
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damageInfo.amount);

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        onDamaged?.Invoke(damageInfo);
        OnDamaged?.Invoke(damageInfo);

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    /// <summary>Applies simple damage with default position/direction.</summary>
    /// <param name="amount">Amount of damage to apply.</param>
    /// <param name="source">GameObject that caused the damage (optional).</param>
    public void TakeDamage(float amount, GameObject source = null)
    {
        TakeDamage(new DamageInfo(amount, transform.position, Vector3.zero, source));
    }

    /// <summary>Heals this object by the specified amount.</summary>
    /// <param name="amount">Amount of health to restore.</param>
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>Sets the current health directly.</summary>
    /// <param name="health">New health value.</param>
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    /// <summary>Sets the maximum health and optionally adjusts current health proportionally.</summary>
    /// <param name="newMaxHealth">New maximum health value.</param>
    /// <param name="adjustCurrent">Whether to adjust current health proportionally.</param>
    public void SetMaxHealth(float newMaxHealth, bool adjustCurrent = true)
    {
        if (adjustCurrent && maxHealth > 0f)
        {
            currentHealth = Mathf.Clamp(currentHealth / maxHealth * newMaxHealth, 0f, newMaxHealth);
        }

        maxHealth = Mathf.Max(1f, newMaxHealth);

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        onDeath?.Invoke();
        OnDeath?.Invoke();
    }

    /// <summary>Revives the object with full health.</summary>
    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>Revives the object with a specific amount of health.</summary>
    /// <param name="health">Health to revive with.</param>
    public void Revive(float health)
    {
        isDead = false;
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);

        onHealthChanged?.Invoke(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}