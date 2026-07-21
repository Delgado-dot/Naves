using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Unity.Cinemachine;


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
    [Header("Death Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private float explosionDuration = 2f;


    [SerializeField] private CinemachineCamera gameplayCamera;

    private bool hasDied;

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
        if (hasDied)
            return;

        hasDied = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.DisableMovement();
        }

        if (gameplayCamera != null)
        {
            gameplayCamera.Follow = null;
            gameplayCamera.LookAt = null;
        }

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosion, explosionDuration);
        }

        if (playerModel != null)
        {
            playerModel.SetActive(false);
        }

        StartCoroutine(DeathSequence());
    }
    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(explosionDuration);

        OnDeath?.Invoke();
    }
}
