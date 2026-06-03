using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    public MyPoolManager PoolManager { get; private set; }
    public CustomSceneManager SceneManager { get; private set; }

    [Header("Pool")]
    [SerializeField] private PoolSettingSo poolSettings;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void TryCreateBeforeAwake()
    {
        if (Instance != null) return;
        GameObject prefab = Resources.Load<GameObject>("GameBootstrapper");
        if (prefab != null)
            Instantiate(prefab);
        else
            Debug.LogWarning("[Bootstrapper] No se encontró el prefab en Resources/.");
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePoolManager();
        InitializeSceneManager();
        InitializeAudioManager();
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    // ── Init Services ───────────────────────────
    private void InitializePoolManager()
    {
        GameObject go = new("Pool Manager");
        go.transform.SetParent(transform);
        PoolManager = go.AddComponent<MyPoolManager>();
        PoolManager.Init(poolSettings);
    }
    private void InitializeSceneManager()
    {
        GameObject go = new("Scene Manager");
        go.transform.SetParent(transform);
        SceneManager = go.AddComponent<CustomSceneManager>();
        SceneManager.Init();
    }
    private void InitializeAudioManager()
    {
        GameObject go = new("Audio Manager");
        go.transform.SetParent(transform);
        AudioManager am = go.AddComponent<AudioManager>();

        am.Setup(musicSource, sfxSource, uiSource);
    }
}
