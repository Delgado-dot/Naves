using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controla el ciclo de rondas/oleadas de la horda.
/// Usa WaveConfig y DifficultyConfig para escalar la dificultad.
/// </summary>
public class RoundManager : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private WaveConfig baseWaveConfig;
    [SerializeField] private DifficultyConfig difficultyConfig;
    [SerializeField] private bool startOnPlay = true;

    [Header("Spawners")]
    [SerializeField] private List<EnemySpawner> spawners = new List<EnemySpawner>();

    [Header("Eventos")]
    [SerializeField] private UnityEvent onRoundStarted;
    [SerializeField] private UnityEvent onRoundEnded;
    [SerializeField] private UnityEvent<int> onNewRound;

    private float timeRemaining;
    private int currentRound;
    private bool isRoundActive;
    private DifficultyValues currentDifficulty;

    public bool IsRoundActive => isRoundActive;
    public float TimeRemaining => timeRemaining;
    public int CurrentRound => currentRound;
    public DifficultyValues CurrentDifficulty => currentDifficulty;

    public event Action<int> RoundStarted;
    public event Action<int> RoundEnded;

    private void Start()
    {
        if (!startOnPlay) return;

        if (baseWaveConfig == null)
        {
            Debug.LogError("[RoundManager] No hay baseWaveConfig asignado!", this);
            return;
        }

        if (difficultyConfig == null)
        {
            Debug.LogError("[RoundManager] No hay difficultyConfig asignado!", this);
            return;
        }

        if (spawners.Count == 0)
        {
            Debug.LogError("[RoundManager] No hay spawners en la lista!", this);
            return;
        }

        StartNewGame();
    }

    private void Update()
    {
    }

    public void StartNewGame()
    {
        currentRound = 0;
        StartNextRound();
    }

    [ContextMenu("Iniciar Siguiente Ronda")]
    public void StartNextRound()
    {
        currentRound++;
        currentDifficulty = difficultyConfig.CalculateForRound(currentRound, baseWaveConfig);

        ResetSpawnerConfigs();
        ApplyDifficultyToSpawners();

        timeRemaining = currentDifficulty.waveDuration;
        isRoundActive = true;

        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }

        RoundStarted?.Invoke(currentRound);
        onRoundStarted?.Invoke();
        onNewRound?.Invoke(currentRound);
    }

    [ContextMenu("Terminar Ronda")]
    public void EndRound()
    {
        if (!isRoundActive) return;

        isRoundActive = false;
        timeRemaining = 0f;

        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
        }

        RoundEnded?.Invoke(currentRound);
        onRoundEnded?.Invoke();
    }

    private void ApplyDifficultyToSpawners()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.ApplyDifficulty(
                    currentDifficulty.spawnInterval,
                    currentDifficulty.maxAlivePerSpawner
                );
            }
        }
    }

    [ContextMenu("Reiniciar Ronda")]
    public void RestartRound()
    {
        EndRound();
        ResetSpawnerConfigs();
        StartNextRound();
    }

    private void ResetSpawnerConfigs()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.ResetConfig(
                    baseWaveConfig.baseSpawnInterval,
                    baseWaveConfig.baseMaxAlivePerSpawner
                );
            }
        }
    }
}
