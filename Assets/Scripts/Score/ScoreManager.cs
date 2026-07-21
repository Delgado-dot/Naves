using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gestor centralizado de puntos.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private int startingScore = 0;

    [Header("Eventos")]
    public UnityEvent<int> OnScoreChanged;

    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    public event Action<int> ScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Score = startingScore;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        Score += amount;
        ScoreChanged?.Invoke(Score);
        OnScoreChanged?.Invoke(Score);
    }

    public void AddScore(float amount)
    {
        AddScore(Mathf.RoundToInt(amount));
    }

    public void ResetScore()
    {
        Score = startingScore;
        ScoreChanged?.Invoke(Score);
        OnScoreChanged?.Invoke(Score);
    }
}
