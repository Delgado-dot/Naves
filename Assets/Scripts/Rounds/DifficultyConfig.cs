using UnityEngine;

/// <summary>
/// Configuracion de escalado de dificultad entre rondas.
/// </summary>
[CreateAssetMenu(menuName = "Naves/Difficulty Config", fileName = "DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Escalado por ronda")]
    [Tooltip("Reduccion del intervalo de spawn por ronda (ej: 0.9 = 10% mas rapido).")]
    [Range(0.5f, 1f)] public float intervalScalePerRound = 0.9f;

    [Tooltip("Enemigos extra vivos simultaneos por ronda.")]
    public int extraAlivePerRound = 1;

    [Tooltip("Aumento de vida de enemigos por ronda (ej: 1.15 = 15% mas vida).")]
    [Min(1f)] public float healthScalePerRound = 1.15f;

    [Tooltip("Aumento de velocidad de enemigos por ronda.")]
    [Min(1f)] public float speedScalePerRound = 1.05f;

    [Tooltip("Duracion extra de la oleada por ronda en segundos.")]
    [Min(0f)] public float extraDurationPerRound = 5f;

    [Header("Tope")]
    [Tooltip("Multiplicador maximo de vida.")]
    public float maxHealthMultiplier = 5f;

    [Tooltip("Intervalo minimo absoluto en segundos.")]
    public float minInterval = 0.3f;

    public DifficultyValues CalculateForRound(int roundNumber, WaveConfig baseConfig)
    {
        int effectiveRound = Mathf.Max(0, roundNumber - 1);

        float interval = baseConfig.baseSpawnInterval * Mathf.Pow(intervalScalePerRound, effectiveRound);
        interval = Mathf.Max(minInterval, interval);

        int maxAlive = baseConfig.baseMaxAlivePerSpawner + (extraAlivePerRound * effectiveRound);

        float healthMult = Mathf.Min(
            baseConfig.enemyHealthMultiplier * Mathf.Pow(healthScalePerRound, effectiveRound),
            maxHealthMultiplier
        );

        float speedMult = baseConfig.enemySpeedMultiplier * Mathf.Pow(speedScalePerRound, effectiveRound);

        float duration = baseConfig.waveDuration + (extraDurationPerRound * effectiveRound);

        return new DifficultyValues
        {
            spawnInterval = interval,
            maxAlivePerSpawner = maxAlive,
            healthMultiplier = healthMult,
            speedMultiplier = speedMult,
            waveDuration = duration
        };
    }
}

public struct DifficultyValues
{
    public float spawnInterval;
    public int maxAlivePerSpawner;
    public float healthMultiplier;
    public float speedMultiplier;
    public float waveDuration;
}
