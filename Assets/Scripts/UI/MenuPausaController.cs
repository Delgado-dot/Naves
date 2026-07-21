using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuPausaController : MonoBehaviour
{
    private const string EscenaMenuPrincipal = "MenuPrincipal";

    [Header("Menú")]
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private Button botonContinuar;
    [SerializeField] private Button botonReiniciar;
    [SerializeField] private Button botonMenuPrincipal;
    [SerializeField] private Button botonSalir;

    private PlayerInputActions inputActions;
    private CursorLockMode cursorAnterior;
    private bool cursorVisibleAnterior;
    private bool pausado;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        ConectarBotones();
        Time.timeScale = 1f;

        if (menuPausa != null)
            menuPausa.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions ??= new PlayerInputActions();
        inputActions.Player.Pause.performed += AlPresionarPausa;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        if (inputActions == null)
            return;

        inputActions.Player.Pause.performed -= AlPresionarPausa;
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        if (pausado)
            Time.timeScale = 1f;

        inputActions?.Dispose();
    }

    private void AlPresionarPausa(InputAction.CallbackContext contexto)
    {
        if (pausado)
            Continuar();
        else
            Pausar();
    }

    public void Pausar()
    {
        if (menuPausa == null || pausado)
            return;

        cursorAnterior = Cursor.lockState;
        cursorVisibleAnterior = Cursor.visible;
        pausado = true;
        menuPausa.SetActive(true);
        menuPausa.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (botonContinuar != null)
            botonContinuar.Select();
    }

    public void Continuar()
    {
        if (!pausado)
            return;

        pausado = false;
        Time.timeScale = 1f;
        menuPausa.SetActive(false);
        Cursor.lockState = cursorAnterior;
        Cursor.visible = cursorVisibleAnterior;
    }

    public void Reiniciar()
    {
        PrepararCambioDeEscena();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenuPrincipal()
    {
        PrepararCambioDeEscena();
        SceneManager.LoadScene(EscenaMenuPrincipal);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PrepararCambioDeEscena()
    {
        pausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ConectarBotones()
    {
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveListener(Continuar);
            botonContinuar.onClick.AddListener(Continuar);
        }

        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.RemoveListener(Reiniciar);
            botonReiniciar.onClick.AddListener(Reiniciar);
        }

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveListener(IrAlMenuPrincipal);
            botonMenuPrincipal.onClick.AddListener(IrAlMenuPrincipal);
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.RemoveListener(Salir);
            botonSalir.onClick.AddListener(Salir);
        }
    }
}
