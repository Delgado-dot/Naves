using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuPrincipalController : MonoBehaviour
{
    private const string EscenaGameplay = "Gameplay";

    [Header("Botones principales")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private Button botonAjustes;
    [SerializeField] private Button botonCreditos;
    [SerializeField] private Button botonSalir;

    [Header("Pantalla de instrucciones")]
    [SerializeField] private PantallaInstrucciones pantallaInstrucciones;

    [Header("Ajustes")]
    [SerializeField] private GameObject panelAjustes;
    [SerializeField] private Slider volumenGeneral;
    [SerializeField] private Toggle pantallaCompleta;
    [SerializeField] private Button botonVolverAjustes;

    [Header("Créditos")]
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private TMP_Text nombresEquipo;
    [SerializeField] private Button botonVolverCreditos;

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ConectarEventos();

        if (volumenGeneral != null)
            volumenGeneral.SetValueWithoutNotify(AudioListener.volume);

        if (pantallaCompleta != null)
            pantallaCompleta.SetIsOnWithoutNotify(Screen.fullScreen);

        CerrarPaneles();
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            CerrarPaneles();
    }

    public void Jugar()
    {
        Time.timeScale = 1f;

        if (pantallaInstrucciones != null)
        {
            pantallaInstrucciones.Mostrar();
        }
        else
        {
            // Respaldo por si olvidaste asignar el script en el Inspector.
            SceneManager.LoadScene(EscenaGameplay);
        }
    }

    public void AbrirAjustes()
    {
        panelCreditos?.SetActive(false);
        panelAjustes?.SetActive(true);
        botonVolverAjustes?.Select();
    }

    public void AbrirCreditos()
    {
        panelAjustes?.SetActive(false);
        panelCreditos?.SetActive(true);
        botonVolverCreditos?.Select();
    }

    public void CerrarPaneles()
    {
        panelAjustes?.SetActive(false);
        panelCreditos?.SetActive(false);
        botonJugar?.Select();
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = Mathf.Clamp01(valor);
    }

    public void CambiarPantallaCompleta(bool activa)
    {
        Screen.fullScreen = activa;
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ConectarEventos()
    {
        Conectar(botonJugar, Jugar);
        Conectar(botonAjustes, AbrirAjustes);
        Conectar(botonCreditos, AbrirCreditos);
        Conectar(botonSalir, Salir);
        Conectar(botonVolverAjustes, CerrarPaneles);
        Conectar(botonVolverCreditos, CerrarPaneles);

        if (volumenGeneral != null)
        {
            volumenGeneral.onValueChanged.RemoveListener(CambiarVolumen);
            volumenGeneral.onValueChanged.AddListener(CambiarVolumen);
        }

        if (pantallaCompleta != null)
        {
            pantallaCompleta.onValueChanged.RemoveListener(CambiarPantallaCompleta);
            pantallaCompleta.onValueChanged.AddListener(CambiarPantallaCompleta);
        }
    }

    private static void Conectar(Button boton, UnityEngine.Events.UnityAction accion)
    {
        if (boton == null)
            return;

        boton.onClick.RemoveListener(accion);
        boton.onClick.AddListener(accion);
    }
}