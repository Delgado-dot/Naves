using UnityEngine;

public enum UISoundType
{
    Confirm,
    Play,
    Panel,
    Exit
}

[RequireComponent(typeof(AudioSource))]
[DefaultExecutionOrder(-200)]
public sealed class UISoundManager : MonoBehaviour
{
    [Header("Clips de interfaz")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip playClip;
    [SerializeField] private AudioClip panelClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioClip confirmClip;

    [Header("Configuracion")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.65f;

    private AudioSource audioSource;

    public static UISoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayHover()
    {
        PlayClip(hoverClip);
    }

    public void Play(UISoundType soundType)
    {
        AudioClip clip = soundType switch
        {
            UISoundType.Play => playClip,
            UISoundType.Panel => panelClip,
            UISoundType.Exit => exitClip,
            _ => confirmClip
        };

        PlayClip(clip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        volume = Mathf.Clamp01(volume);
        AudioSource source = GetComponent<AudioSource>();
        if (source == null)
            return;

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = volume;
    }
#endif
}
