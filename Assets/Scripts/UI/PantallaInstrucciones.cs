using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaInstrucciones : MonoBehaviour
{
    [SerializeField] private GameObject panelInstrucciones;
    [SerializeField] private Image fade;

    [SerializeField] private string escenaGameplay = "Gameplay";
    [SerializeField] private float velocidadFade = 2f;

    private bool esperandoEnter = false;

    public void Mostrar()
    {
        panelInstrucciones.SetActive(true);
        esperandoEnter = true;
    }

    private void Update()
    {
        if (!esperandoEnter)
            return;

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            esperandoEnter = false;
            StartCoroutine(FadeYCargar());
        }
    }

    IEnumerator FadeYCargar()
    {
        Color c = fade.color;
        c.a = 0;
        fade.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            fade.color = c;
            yield return null;
        }

        SceneManager.LoadScene(escenaGameplay);
    }
}