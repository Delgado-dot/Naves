using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sonido
    {
        public string nombre;
        public AudioClip clip;
        public KeyCode tecla = KeyCode.None;

        [Range(0f, 1f)]
        public float volumen = 1f;
    }


    [Header("Música de fondo")]
    public AudioSource musicaSource;
    public AudioClip musicaFondo;

    [Range(0f, 1f)]
    public float volumenMusica = 0.5f;


    [Header("Efectos de sonido")]
    public AudioSource efectosSource;

    public Sonido[] sonidos;


    void Start()
    {
        if (musicaSource != null && musicaFondo != null)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.volume = volumenMusica;
            musicaSource.Play();
        }
    }


    void Update()
    {
        foreach (Sonido sonido in sonidos)
        {
            if (sonido.clip != null &&
                sonido.tecla != KeyCode.None &&
                Input.GetKeyDown(sonido.tecla))
            {
                efectosSource.PlayOneShot(sonido.clip, sonido.volumen);
            }
        }
    }


    public void Reproducir(string nombre)
    {
        foreach (Sonido sonido in sonidos)
        {
            if (sonido.nombre == nombre)
            {
                efectosSource.PlayOneShot(sonido.clip, sonido.volumen);
                return;
            }
        }
    }


    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (musicaSource == null || nuevaMusica == null)
            return;

        musicaSource.Stop();

        musicaSource.clip = nuevaMusica;
        musicaSource.loop = true;
        musicaSource.volume = volumenMusica;
        musicaSource.Play();
    }


    public void DetenerMusica()
    {
        if (musicaSource != null)
            musicaSource.Stop();
    }


    public void ReanudarMusica()
    {
        if (musicaSource != null && !musicaSource.isPlaying)
            musicaSource.Play();
    }
}