using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("-------- Audio Source --------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Music")]
    public AudioClip backgroundMusic;

    private void Awake()
    {
        // Destroying duplicates and setting up singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
    private void Start()
    {
        // Play background music on start
        if (musicSource == null || backgroundMusic == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }
    // ── Music ──────────────────────────────────────────
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        musicSource.clip = null;
    }
    // ── SFX ─────────────────────────────────────────────
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
    public void PlayLoopedSFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.clip = clip;
        sfxSource.loop = true;
        sfxSource.Play();
    }
    public void StopLoopedSFX()
    {
        if (sfxSource == null) return;
        sfxSource.loop = false;
        sfxSource.Stop();
    }
    // ── UI ──────────────────────────────────────────────
    public void PlayUI(AudioClip clip)
    {
        if (uiSource == null || clip == null) return;
        uiSource.PlayOneShot(clip);
    }
    // ── Stop All ─────────────────────────────────────────
    public void StopAll()
    {
        StopMusic();
        StopLoopedSFX();
        uiSource?.Stop();
    }
}