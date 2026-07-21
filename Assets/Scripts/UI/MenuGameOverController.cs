using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteAlways]
public sealed class MenuGameOverController : MonoBehaviour
{
    private const string EscenaGameplay = "Gameplay";
    private const string EscenaMenuPrincipal = "MenuPrincipal";

    [Header("Menu Game Over")]
    [SerializeField] private GameObject menuGameOver;
    [SerializeField, Min(0f)] private float demoraTrasExplosion = 1.15f;

    private PlayerHealth playerHealth;
    private HUDSuperiorController hudSuperior;
    private RoundManager roundManager;
    private TMP_Text textoPuntos;
    private TMP_Text textoHorda;
    private TMP_Text textoTiempo;
    private Button botonReintentar;
    private int puntosFinales;
    private int hordaFinal;
    private float tiempoFinal;
    private bool gameOverSolicitado;

    public static bool GameOverActivo { get; private set; }

    private void Awake()
    {
        if (menuGameOver != null)
        {
            ConstruirInterfazSiEsNecesario();
            AplicarTransparencia();
        }

        if (!Application.isPlaying)
            return;

        if (menuGameOver != null)
            menuGameOver.SetActive(false);

        GameOverActivo = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        playerHealth = FindFirstObjectByType<PlayerHealth>();
        hudSuperior = FindFirstObjectByType<HUDSuperiorController>();
        roundManager = FindFirstObjectByType<RoundManager>();

        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(AlMorirJugador);
        else
            Debug.LogError("[MenuGameOver] No se encontro PlayerHealth en Gameplay.", this);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(AlMorirJugador);

        if (GameOverActivo)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        GameOverActivo = false;
    }

    private void AlMorirJugador()
    {
        if (gameOverSolicitado)
            return;

        gameOverSolicitado = true;
        GameOverActivo = true;
        puntosFinales = ScoreManager.Instance != null ? Mathf.Max(0, ScoreManager.Instance.Score) : 0;
        hordaFinal = roundManager != null ? Mathf.Max(1, roundManager.CurrentRound) : hudSuperior != null ? hudSuperior.HordaActual : 1;
        tiempoFinal = hudSuperior != null ? hudSuperior.TiempoSobrevivido : Time.timeSinceLevelLoad;
        StartCoroutine(MostrarDespuesDeExplosion());
    }

    private IEnumerator MostrarDespuesDeExplosion()
    {
        if (demoraTrasExplosion > 0f)
            yield return new WaitForSeconds(demoraTrasExplosion);

        MostrarGameOver();
    }

    private void MostrarGameOver()
    {
        if (menuGameOver == null)
            return;

        ActualizarResultados();
        GameOverActivo = true;
        menuGameOver.SetActive(true);
        menuGameOver.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        botonReintentar?.Select();
    }

    public void Reintentar()
    {
        PrepararCambioDeEscena();
        SceneManager.LoadScene(EscenaGameplay);
    }

    public void IrAlMenuPrincipal()
    {
        PrepararCambioDeEscena();
        SceneManager.LoadScene(EscenaMenuPrincipal);
    }

    public void Salir()
    {
        PrepararCambioDeEscena();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PrepararCambioDeEscena()
    {
        GameOverActivo = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ActualizarResultados()
    {
        if (textoPuntos != null)
            textoPuntos.text = $"PUNTOS: {puntosFinales:0000}";
        if (textoHorda != null)
            textoHorda.text = $"HORDA: {hordaFinal}";
        if (textoTiempo != null)
        {
            int totalSegundos = Mathf.Max(0, Mathf.FloorToInt(tiempoFinal));
            textoTiempo.text = $"TIEMPO: {totalSegundos / 60:00}:{totalSegundos % 60:00}";
        }
    }

    private void ConstruirInterfazSiEsNecesario()
    {
        if (menuGameOver.transform.Find("FondoOscuro") != null)
            return;

        RectTransform raiz = menuGameOver.GetComponent<RectTransform>();
        AjustarRect(raiz, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image fondo = CrearImagen("FondoOscuro", menuGameOver.transform, new Color(0.005f, 0.008f, 0.025f, 0.22f));
        AjustarRect(fondo.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image marco = CrearImagen("MarcoCian", fondo.transform, new Color(0.03f, 0.78f, 1f, 0.96f));
        AjustarCentro(marco.rectTransform, new Vector2(760f, 790f), Vector2.zero);

        Image panel = CrearImagen("PanelAzulOscuro", marco.transform, new Color(0.006f, 0.025f, 0.08f, 0.72f));
        AjustarRect(panel.rectTransform, Vector2.zero, Vector2.one, new Vector2(6f, 6f), new Vector2(-6f, -6f));

        Image bordeVioleta = CrearImagen("DetalleVioleta", panel.transform, new Color(0.55f, 0.16f, 0.95f, 0.9f));
        AjustarCentro(bordeVioleta.rectTransform, new Vector2(5f, 610f), new Vector2(350f, 0f));

        TMP_Text tituloBrillo = CrearTexto("BrilloGameOver", panel.transform, "GAME OVER", 76f, new Color(1f, 0.05f, 0.03f, 0.34f));
        AjustarCentro(tituloBrillo.rectTransform, new Vector2(650f, 120f), new Vector2(0f, 282f));
        tituloBrillo.fontStyle = FontStyles.Bold;
        tituloBrillo.transform.localScale = Vector3.one * 1.045f;

        TMP_Text titulo = CrearTexto("TituloGameOver", panel.transform, "GAME OVER", 72f, new Color(1f, 0.93f, 0.93f, 1f));
        AjustarCentro(titulo.rectTransform, new Vector2(650f, 120f), new Vector2(0f, 282f));
        titulo.fontStyle = FontStyles.Bold;
        titulo.outlineColor = new Color32(210, 18, 18, 255);
        titulo.outlineWidth = 0.3f;

        Image separador = CrearImagen("SeparadorRojo", panel.transform, new Color(1f, 0.12f, 0.08f, 0.9f));
        AjustarCentro(separador.rectTransform, new Vector2(520f, 3f), new Vector2(0f, 220f));

        textoPuntos = CrearFilaResultado(panel.transform, "FilaPuntos", "PUNTOS: 0000", 135f);
        textoHorda = CrearFilaResultado(panel.transform, "FilaHorda", "HORDA: 1", 55f);
        textoTiempo = CrearFilaResultado(panel.transform, "FilaTiempo", "TIEMPO: 00:00", -25f);

        botonReintentar = CrearBoton(panel.transform, "BotonReintentar", "REINTENTAR", -145f, Reintentar, true);
        CrearBoton(panel.transform, "BotonMenuPrincipal", "MENÚ PRINCIPAL", -235f, IrAlMenuPrincipal, false);
        CrearBoton(panel.transform, "BotonSalir", "SALIR", -325f, Salir, false);
    }

    private void AplicarTransparencia()
    {
        Transform fondoTransform = menuGameOver.transform.Find("FondoOscuro");
        if (fondoTransform == null)
            return;

        Image fondo = fondoTransform.GetComponent<Image>();
        if (fondo != null)
            fondo.color = new Color(0.005f, 0.008f, 0.025f, 0.22f);

        Transform panelTransform = fondoTransform.Find("MarcoCian/PanelAzulOscuro");
        Image panel = panelTransform != null ? panelTransform.GetComponent<Image>() : null;
        if (panel != null)
            panel.color = new Color(0.006f, 0.025f, 0.08f, 0.72f);
    }

    private TMP_Text CrearFilaResultado(Transform padre, string nombre, string contenido, float y)
    {
        Image borde = CrearImagen(nombre, padre, new Color(0.04f, 0.42f, 0.68f, 0.9f));
        AjustarCentro(borde.rectTransform, new Vector2(610f, 64f), new Vector2(0f, y));
        Image interior = CrearImagen("Fondo", borde.transform, new Color(0.006f, 0.02f, 0.055f, 1f));
        AjustarRect(interior.rectTransform, Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
        TMP_Text texto = CrearTexto("Texto", interior.transform, contenido, 32f, new Color(0.25f, 0.82f, 1f, 1f));
        AjustarRect(texto.rectTransform, Vector2.zero, Vector2.one, new Vector2(28f, 0f), new Vector2(-28f, 0f));
        texto.alignment = TextAlignmentOptions.MidlineLeft;
        texto.fontStyle = FontStyles.Bold;
        return texto;
    }

    private Button CrearBoton(Transform padre, string nombre, string etiqueta, float y, UnityEngine.Events.UnityAction accion, bool destacado)
    {
        Color colorBorde = destacado ? new Color(0.1f, 0.9f, 1f, 1f) : new Color(0.08f, 0.48f, 0.86f, 0.95f);
        Image borde = CrearImagen(nombre, padre, colorBorde);
        AjustarCentro(borde.rectTransform, new Vector2(520f, 66f), new Vector2(0f, y));
        Button boton = borde.gameObject.AddComponent<Button>();
        boton.targetGraphic = borde;
        boton.transition = Selectable.Transition.ColorTint;
        ColorBlock colores = boton.colors;
        colores.normalColor = Color.white;
        colores.highlightedColor = new Color(0.72f, 0.95f, 1f, 1f);
        colores.pressedColor = new Color(0.5f, 0.7f, 1f, 1f);
        colores.selectedColor = colores.highlightedColor;
        boton.colors = colores;
        boton.onClick.AddListener(accion);

        Image interior = CrearImagen("Fondo", borde.transform, destacado
            ? new Color(0.015f, 0.16f, 0.27f, 1f)
            : new Color(0.006f, 0.025f, 0.075f, 1f));
        AjustarRect(interior.rectTransform, Vector2.zero, Vector2.one, new Vector2(3f, 3f), new Vector2(-3f, -3f));
        interior.raycastTarget = false;
        TMP_Text texto = CrearTexto("Texto", interior.transform, etiqueta, 30f, Color.white);
        AjustarRect(texto.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        texto.fontStyle = FontStyles.Bold;
        return boton;
    }

    private static Image CrearImagen(string nombre, Transform padre, Color color)
    {
        GameObject objeto = new(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        objeto.layer = padre.gameObject.layer;
        objeto.transform.SetParent(padre, false);
        Image imagen = objeto.GetComponent<Image>();
        imagen.color = color;
        return imagen;
    }

    private static TMP_Text CrearTexto(string nombre, Transform padre, string contenido, float tamano, Color color)
    {
        GameObject objeto = new(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        objeto.layer = padre.gameObject.layer;
        objeto.transform.SetParent(padre, false);
        TextMeshProUGUI texto = objeto.GetComponent<TextMeshProUGUI>();
        texto.text = contenido;
        texto.fontSize = tamano;
        texto.color = color;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        texto.textWrappingMode = TextWrappingModes.NoWrap;
        return texto;
    }

    private static void AjustarCentro(RectTransform rect, Vector2 tamano, Vector2 posicion)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = tamano;
        rect.anchoredPosition = posicion;
    }

    private static void AjustarRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
