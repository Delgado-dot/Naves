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
    private const string MarcaDiseno = "DisenoReferenciaV3";

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
            textoPuntos.text = $"{puntosFinales:0000}";
        if (textoHorda != null)
            textoHorda.text = $"{hordaFinal}";
        if (textoTiempo != null)
        {
            int totalSegundos = Mathf.Max(0, Mathf.FloorToInt(tiempoFinal));
            textoTiempo.text = $"{totalSegundos / 60:00}:{totalSegundos % 60:00}";
        }
    }

    private void ConstruirInterfazSiEsNecesario()
    {
        Transform disenoExistente = menuGameOver.transform.Find($"FondoOscuro/PanelPrincipal/{MarcaDiseno}");
        if (disenoExistente != null)
        {
            ConectarInterfazExistente();
            return;
        }

        Transform fondoAnterior = menuGameOver.transform.Find("FondoOscuro");
        if (fondoAnterior != null)
        {
            if (Application.isPlaying)
                Destroy(fondoAnterior.gameObject);
            else
                DestroyImmediate(fondoAnterior.gameObject);
        }

        RectTransform raiz = menuGameOver.GetComponent<RectTransform>();
        AjustarRect(raiz, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image fondo = CrearImagen("FondoOscuro", menuGameOver.transform, new Color(0.002f, 0.006f, 0.02f, 0.28f));
        AjustarRect(fondo.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image panel = CrearImagen("PanelPrincipal", fondo.transform, new Color(0.003f, 0.014f, 0.045f, 0.9f));
        AjustarCentro(panel.rectTransform, new Vector2(720f, 690f), Vector2.zero);

        GameObject marca = new(MarcaDiseno, typeof(RectTransform));
        marca.layer = panel.gameObject.layer;
        marca.transform.SetParent(panel.transform, false);

        CrearMarcoTecnologico(panel.transform);

        CrearDetalle(panel.transform, "SuperiorIzquierda", new Vector2(135f, 5f), new Vector2(-270f, 327f), new Color(0.05f, 0.88f, 1f, 1f));
        CrearDetalle(panel.transform, "SuperiorDerecha", new Vector2(135f, 5f), new Vector2(270f, 327f), new Color(0.05f, 0.88f, 1f, 1f));
        CrearDetalle(panel.transform, "LateralRojo", new Vector2(5f, 145f), new Vector2(-346f, 45f), new Color(1f, 0.16f, 0.08f, 1f));
        CrearDetalle(panel.transform, "LateralVioleta", new Vector2(5f, 190f), new Vector2(346f, -40f), new Color(0.56f, 0.18f, 1f, 1f));

        TMP_Text tituloBrillo = CrearTexto("BrilloGameOver", panel.transform, "GAME OVER", 78f, new Color(1f, 0.04f, 0.02f, 0.38f));
        AjustarCentro(tituloBrillo.rectTransform, new Vector2(620f, 112f), new Vector2(0f, 260f));
        tituloBrillo.fontStyle = FontStyles.Bold;
        tituloBrillo.transform.localScale = Vector3.one * 1.06f;

        TMP_Text titulo = CrearTexto("TituloGameOver", panel.transform, "GAME OVER", 72f, new Color(1f, 0.96f, 0.95f, 1f));
        AjustarCentro(titulo.rectTransform, new Vector2(620f, 112f), new Vector2(0f, 260f));
        titulo.fontStyle = FontStyles.Bold;
        titulo.outlineColor = new Color32(235, 24, 18, 255);
        titulo.outlineWidth = 0.34f;

        Image separador = CrearImagen("SeparadorRojo", panel.transform, new Color(1f, 0.12f, 0.08f, 0.9f));
        AjustarCentro(separador.rectTransform, new Vector2(510f, 3f), new Vector2(0f, 202f));

        CrearFilaResultado(panel.transform, "FilaPuntos", "PUNTOS", "0000", 125f, out textoPuntos);
        CrearFilaResultado(panel.transform, "FilaHorda", "HORDA", "1", 55f, out textoHorda);
        CrearFilaResultado(panel.transform, "FilaTiempo", "TIEMPO", "00:00", -15f, out textoTiempo);

        botonReintentar = CrearBoton(panel.transform, "BotonReintentar", "REINTENTAR", -135f, Reintentar, true);
        CrearBoton(panel.transform, "BotonMenuPrincipal", "MENÚ PRINCIPAL", -210f, IrAlMenuPrincipal, false);
        CrearBoton(panel.transform, "BotonSalir", "SALIR", -285f, Salir, false);
    }

    private void AplicarTransparencia()
    {
        Transform fondoTransform = menuGameOver.transform.Find("FondoOscuro");
        if (fondoTransform == null)
            return;

        Image fondo = fondoTransform.GetComponent<Image>();
        if (fondo != null)
            fondo.color = new Color(0.002f, 0.006f, 0.02f, 0.28f);

        Transform panelTransform = fondoTransform.Find("PanelPrincipal");
        Image panel = panelTransform != null ? panelTransform.GetComponent<Image>() : null;
        if (panel != null)
            panel.color = new Color(0.003f, 0.014f, 0.045f, 0.9f);
    }

    private void CrearFilaResultado(Transform padre, string nombre, string etiqueta, string valorInicial, float y, out TMP_Text textoValor)
    {
        Image borde = CrearImagen(nombre, padre, new Color(0.04f, 0.42f, 0.68f, 0.9f));
        AjustarCentro(borde.rectTransform, new Vector2(610f, 58f), new Vector2(0f, y));
        Image interior = CrearImagen("Fondo", borde.transform, new Color(0.004f, 0.018f, 0.05f, 0.9f));
        AjustarRect(interior.rectTransform, Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));

        TMP_Text textoEtiqueta = CrearTexto("Etiqueta", interior.transform, etiqueta, 29f, Color.white);
        AjustarRect(textoEtiqueta.rectTransform, Vector2.zero, Vector2.one, new Vector2(38f, 0f), new Vector2(-300f, 0f));
        textoEtiqueta.alignment = TextAlignmentOptions.MidlineLeft;
        textoEtiqueta.fontStyle = FontStyles.Bold;

        textoValor = CrearTexto("Valor", interior.transform, valorInicial, 38f, new Color(0.05f, 0.72f, 1f, 1f));
        AjustarRect(textoValor.rectTransform, Vector2.zero, Vector2.one, new Vector2(330f, 0f), new Vector2(-42f, 0f));
        textoValor.alignment = TextAlignmentOptions.MidlineRight;
        textoValor.fontStyle = FontStyles.Bold;
    }

    private void ConectarInterfazExistente()
    {
        Transform panel = menuGameOver.transform.Find("FondoOscuro/PanelPrincipal");
        if (panel == null)
            return;

        textoPuntos = panel.Find("FilaPuntos/Fondo/Valor")?.GetComponent<TMP_Text>();
        textoHorda = panel.Find("FilaHorda/Fondo/Valor")?.GetComponent<TMP_Text>();
        textoTiempo = panel.Find("FilaTiempo/Fondo/Valor")?.GetComponent<TMP_Text>();
        botonReintentar = ConectarBotonExistente(panel, "BotonReintentar", Reintentar);
        ConectarBotonExistente(panel, "BotonMenuPrincipal", IrAlMenuPrincipal);
        ConectarBotonExistente(panel, "BotonSalir", Salir);
    }

    private static Button ConectarBotonExistente(Transform panel, string nombre, UnityEngine.Events.UnityAction accion)
    {
        Button boton = panel.Find(nombre)?.GetComponent<Button>();
        if (boton == null)
            return null;

        boton.onClick.RemoveListener(accion);
        boton.onClick.AddListener(accion);
        return boton;
    }

    private static void CrearDetalle(Transform padre, string nombre, Vector2 tamano, Vector2 posicion, Color color)
    {
        Image detalle = CrearImagen(nombre, padre, color);
        AjustarCentro(detalle.rectTransform, tamano, posicion);
        detalle.raycastTarget = false;
    }

    private static void CrearMarcoTecnologico(Transform panel)
    {
        Color cyan = new(0.02f, 0.82f, 1f, 1f);
        Color cyanInterior = new(0.04f, 0.38f, 0.68f, 0.9f);
        Color violeta = new(0.5f, 0.18f, 1f, 0.95f);

        CrearLineaMarco(panel, "BordeSuperior", new Vector2(620f, 4f), new Vector2(0f, 343f), 0f, cyan);
        CrearLineaMarco(panel, "BordeInferior", new Vector2(620f, 4f), new Vector2(0f, -343f), 0f, cyan);
        CrearLineaMarco(panel, "BordeIzquierdo", new Vector2(4f, 590f), new Vector2(-358f, 0f), 0f, cyan);
        CrearLineaMarco(panel, "BordeDerecho", new Vector2(4f, 590f), new Vector2(358f, 0f), 0f, cyan);

        CrearLineaMarco(panel, "EsquinaSupIzq", new Vector2(82f, 4f), new Vector2(-329f, 316f), -43f, cyan);
        CrearLineaMarco(panel, "EsquinaSupDer", new Vector2(82f, 4f), new Vector2(329f, 316f), 43f, cyan);
        CrearLineaMarco(panel, "EsquinaInfIzq", new Vector2(82f, 4f), new Vector2(-329f, -316f), 43f, cyan);
        CrearLineaMarco(panel, "EsquinaInfDer", new Vector2(82f, 4f), new Vector2(329f, -316f), -43f, cyan);

        CrearLineaMarco(panel, "InteriorSuperior", new Vector2(510f, 2f), new Vector2(0f, 330f), 0f, cyanInterior);
        CrearLineaMarco(panel, "InteriorInferior", new Vector2(510f, 2f), new Vector2(0f, -330f), 0f, cyanInterior);
        CrearLineaMarco(panel, "AcentoVioletaSuperior", new Vector2(105f, 4f), new Vector2(250f, 337f), 0f, violeta);
        CrearLineaMarco(panel, "AcentoVioletaInferior", new Vector2(105f, 4f), new Vector2(-250f, -337f), 0f, violeta);
    }

    private static void CrearLineaMarco(Transform padre, string nombre, Vector2 tamano, Vector2 posicion, float rotacion, Color color)
    {
        Image linea = CrearImagen(nombre, padre, color);
        AjustarCentro(linea.rectTransform, tamano, posicion);
        linea.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotacion);
        linea.raycastTarget = false;
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
