using UnityEngine;

/// <summary>
/// Configuracion de una oleada como ScriptableObject.
/// </summary>
[CreateAssetMenu(menuName = "Naves/Wave Config", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Header("Duracion")]
    [Tooltip("Duracion de la oleada en segundos.")]
    [Min(5f)] public float waveDuration = 60f;

    [Header("Enemigos")]
    [Tooltip("Cantidad total de enemigos (0 = infinito).")]
    public int totalEnemies = 0;

    [Tooltip("Intervalo base entre spawns en segundos.")]
    [Min(0.1f)] public float baseSpawnInterval = 3f;

    [Tooltip("Maximo de enemigos vivos simultaneos por spawner.")]
    [Min(1)] public int baseMaxAlivePerSpawner = 4;

    [Header("Dificultad")]
    [Tooltip("Multiplicador de vida de los enemigos.")]
    [Min(0.1f)] public float enemyHealthMultiplier = 1f;

    [Tooltip("Multiplicador de velocidad de los enemigos.")]
    [Min(0.1f)] public float enemySpeedMultiplier = 1f;
}
