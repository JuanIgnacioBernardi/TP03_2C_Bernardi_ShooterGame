using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GameOverScreen : MonoBehaviour
{
    public enum ScreenType { Win, Death }

    [Header("Type")]
    [SerializeField] private ScreenType screenType;

    [Header("Panels")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject settingsMenu;

    [Header("Buttons")]
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button settingsBackBtn;

    [Header("Sounds")]
    [SerializeField] private AudioClip hoverUI;
    private void Awake()
    {
        retryBtn.onClick.AddListener(OnRetry);
        settingsBtn.onClick.AddListener(OnSettings);
        homeBtn.onClick.AddListener(OnHome);
        settingsBackBtn.onClick.AddListener(OnSettingsBack);

        AddHoverSound(retryBtn);
        AddHoverSound(settingsBtn);
        AddHoverSound(homeBtn);
        AddHoverSound(settingsBackBtn);
    }
    private void OnDestroy()
    {
        retryBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        settingsBackBtn.onClick.RemoveAllListeners();
    }
    private void OnEnable()
    {
        if (screenType == ScreenType.Win)
            GameEvents.onWin += Show;
        else
            GameEvents.onDeath += Show;
    }
    private void OnDisable()
    {
        if (screenType == ScreenType.Win)
            GameEvents.onWin -= Show;
        else
            GameEvents.onDeath -= Show;
    }
    private void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameEvents.RaisePauseChanged(true);
    }
    private void OnRetry()
    {
        Time.timeScale = 1f;
        GameEvents.IsGameOver = false;
        GameBootstrapper.Instance.SceneManager.GoToGameplay();
        GameBootstrapper.Instance.SceneManager.ActivateScene();
    }
    private void OnSettings() => settingsMenu.SetActive(true);
    private void OnHome()
    {
        Time.timeScale = 1f;
        GameEvents.IsGameOver = false;
        GameBootstrapper.Instance.SceneManager.GoToMainMenu();
        GameBootstrapper.Instance.SceneManager.ActivateScene();
    }
    private void OnSettingsBack() => settingsMenu.SetActive(false);
    private void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>()
                            ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(_ => AudioEvents.RaisePlayUI(hoverUI));
        trigger.triggers.Add(entry);
    }
}