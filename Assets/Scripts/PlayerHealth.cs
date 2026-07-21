using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Player Reference")]
    [SerializeField] private PlayerReferenceSO playerReference;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    private float invincibilityTimer;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsInvincible => invincibilityTimer > 0f;
    public bool IsDead => currentHealth <= 0f;

    private void Start()
    {
        currentHealth = maxHealth;

        if (playerReference != null)
        {
            playerReference.Register(transform);
            Debug.Log($"[PlayerHealth] Jugador registrado en PlayerRef");
        }
        else
        {
            Debug.LogError("[PlayerHealth] Player Reference no asignado en el Inspector!", this);
        }
    }

    private void OnDestroy()
    {
        if (playerReference != null)
        {
            playerReference.Clear();
        }
    }

    private void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || IsInvincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        invincibilityTimer = invincibilityDuration;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamage?.Invoke();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
