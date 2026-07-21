using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Punto de spawn de enemigos. Controla intervalo, maximo de vivos
/// y ciclo Start/Stop manejado por el RoundManager.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab y objetivo")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private PlayerReferenceSO playerReference;

    [Header("Ritmo de aparicion")]
    [Tooltip("Segundos de espera entre cada intento de spawn.")]
    [SerializeField, Min(0.1f)] private float spawnInterval = 8f;

    [Tooltip("Maximo de enemigos vivos generados por este spawner simultaneamente.")]
    [SerializeField, Min(1)] private int maxAliveEnemies = 2;

    [Header("Spawn position")]
    [Tooltip("Radio de aleatoriedad alrededor del punto de spawn.")]
    [SerializeField] private float spawnRadius = 10f;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();
    private Coroutine spawnRoutine;

    public bool IsSpawning { get; private set; }

    public void StartSpawning()
    {
        if (IsSpawning) return;

        IsSpawning = true;
        Debug.Log($"[{name}] StartSpawning activado. Intervalo: {spawnInterval}, Max vivos: {maxAliveEnemies}");
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        IsSpawning = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public void ClearAliveEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] != null)
            {
                Destroy(aliveEnemies[i].gameObject);
            }
        }

        aliveEnemies.Clear();
    }

    public void ApplyDifficulty(float newInterval, int newMaxAlive)
    {
        spawnInterval = Mathf.Max(0.1f, newInterval);
        maxAliveEnemies = Mathf.Max(1, newMaxAlive);
    }

    public void ResetConfig(float baseInterval, int baseMaxAlive)
    {
        spawnInterval = baseInterval;
        maxAliveEnemies = baseMaxAlive;
    }

    private IEnumerator SpawnLoop()
    {
        Debug.Log($"[{name}] SpawnLoop iniciado");
        var wait = new WaitForSeconds(spawnInterval);

        while (IsSpawning)
        {
            Debug.Log($"[{name}] Tick - vivos: {aliveEnemies.Count}/{maxAliveEnemies}");
            if (aliveEnemies.Count < maxAliveEnemies)
            {
                TrySpawnEnemy();
            }

            yield return wait;
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"[{name}] No hay enemyPrefab asignado.", this);
            return;
        }

        if (playerReference == null)
        {
            Debug.LogWarning($"[{name}] PlayerReference no asignado.", this);
            return;
        }

        if (playerReference.PlayerTransform == null)
        {
            Debug.LogWarning($"[{name}] PlayerTransform es null. El asset PlayerRef no tiene el jugador registrado.", this);
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        Debug.Log($"[{name}] Spawneando en {spawnPosition}");
        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        enemy.Initialize(playerReference.PlayerTransform);
        enemy.Died += HandleEnemyDied;

        aliveEnemies.Add(enemy);
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        enemy.Died -= HandleEnemyDied;
        aliveEnemies.Remove(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        return transform.position + randomOffset;
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
