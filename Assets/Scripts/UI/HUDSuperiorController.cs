using TMPro;
using UnityEngine;

public sealed class HUDSuperiorController : MonoBehaviour
{
    [Header("Referencias del HUD superior")]
    [SerializeField] private TMP_Text textoHorda;
    [SerializeField] private TMP_Text textoCronometro;
    [SerializeField] private TMP_Text textoPuntaje;

    [Header("Configuracion")]
    [SerializeField, Min(1f)] private float duracionHorda = 45f;

    private float tiempoSobrevivido;
    private int horda = 1;
    private int puntaje;
    private RoundManager roundManager;

    public float TiempoSobrevivido => tiempoSobrevivido;
    public int HordaActual => horda;
    public int PuntajeActual => puntaje;

    private void Awake()
    {
        roundManager = FindFirstObjectByType<RoundManager>();
        ActualizarHUD();
    }

    private void Update()
    {
        tiempoSobrevivido += Time.deltaTime;

        horda = 1 + Mathf.FloorToInt(tiempoSobrevivido / duracionHorda);

        if (ScoreManager.Instance != null)
            puntaje = Mathf.Max(0, ScoreManager.Instance.Score);

        ActualizarHUD();
    }

    public void SumarPuntos(int cantidad)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(cantidad);
            puntaje = Mathf.Max(0, ScoreManager.Instance.Score);
        }
        else
        {
            puntaje = Mathf.Max(0, puntaje + cantidad);
        }

        ActualizarPuntaje();
    }

    private void ActualizarHUD()
    {
        int segundosTotales = Mathf.FloorToInt(tiempoSobrevivido);
        int minutos = segundosTotales / 60;
        int segundos = segundosTotales % 60;

        textoCronometro.text = $"{minutos:00}:{segundos:00}";
        textoHorda.text = $"HORDA {horda}";
        ActualizarPuntaje();
    }

    private void ActualizarPuntaje()
    {
        textoPuntaje.text = $"PUNTOS {puntaje:0000}";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        duracionHorda = Mathf.Max(1f, duracionHorda);
    }
#endif
}
