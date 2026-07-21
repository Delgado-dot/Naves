using UnityEngine;

public sealed class AnimacionFondoMenu : MonoBehaviour
{
    public enum TipoMovimiento { Flotar, Rotar, Derivar, Camara }

    [SerializeField] private TipoMovimiento tipo = TipoMovimiento.Flotar;
    [SerializeField] private Vector3 eje = Vector3.up;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float amplitud = 0.15f;
    [SerializeField] private float desfase;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    private void Awake()
    {
        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;
        desfase += transform.GetSiblingIndex() * 0.73f;
    }

    private void Update()
    {
        float tiempo = Time.unscaledTime + desfase;
        switch (tipo)
        {
            case TipoMovimiento.Rotar:
                transform.Rotate(eje.normalized, velocidad * Time.unscaledDeltaTime, Space.Self);
                break;
            case TipoMovimiento.Derivar:
                transform.localPosition = posicionInicial + eje.normalized * Mathf.Sin(tiempo * velocidad) * amplitud;
                transform.localRotation = rotacionInicial * Quaternion.Euler(0f, Mathf.Sin(tiempo * .35f) * 4f, 0f);
                break;
            case TipoMovimiento.Camara:
                transform.localPosition = posicionInicial + new Vector3(
                    Mathf.Sin(tiempo * velocidad) * amplitud,
                    Mathf.Cos(tiempo * velocidad * .7f) * amplitud * .45f, 0f);
                break;
            default:
                transform.localPosition = posicionInicial + eje.normalized * Mathf.Sin(tiempo * velocidad) * amplitud;
                transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, Mathf.Sin(tiempo * .8f) * 1.2f);
                break;
        }
    }
}
