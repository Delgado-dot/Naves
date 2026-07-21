using TMPro;
using UnityEngine;

public sealed class HUDInferiorController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private HUDHealthBarGraphic barraVida;
    [SerializeField] private TMP_Text textoVida;

    private void OnEnable()
    {
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(ActualizarVida);
            ActualizarVida(playerHealth.CurrentHealth > 0f ? playerHealth.CurrentHealth : playerHealth.MaxHealth, playerHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged.RemoveListener(ActualizarVida);
    }

    private void ActualizarVida(float actual, float maxima)
    {
        float porcentaje = maxima > 0f ? Mathf.Clamp01(actual / maxima) : 0f;
        barraVida.SetFill(porcentaje);
        textoVida.text = $"VIDA {Mathf.CeilToInt(actual)}";
    }
}
