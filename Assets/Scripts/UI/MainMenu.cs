using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Canvas references")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject creditsCanvas;

    [Header("GameObjects references")]
    [SerializeField] private GameObject main;

    [Header("Buttons reference")]
    [SerializeField] private Button playBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingsBtnBack;
    [SerializeField] private Button creditsBtnBack;

    [Header("Sound references")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;
    private void Awake()
    {
        playBtn.onClick.AddListener(PlayGame);
        settingsBtn.onClick.AddListener(OpenSettings);
        creditsBtn.onClick.AddListener(OpenCredits);
        quitBtn.onClick.AddListener(QuitGame);
        settingsBtnBack.onClick.AddListener(CloseSettings);
        creditsBtnBack.onClick.AddListener(CloseCredits);
        AddHoverSound(playBtn);
        AddHoverSound(settingsBtn);
        AddHoverSound(creditsBtn);
        AddHoverSound(quitBtn);
        AddHoverSound(settingsBtnBack);
        AddHoverSound(creditsBtnBack);
    }
    private void OnDestroy()
    {
        playBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        creditsBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        settingsBtnBack.onClick.RemoveAllListeners();
        creditsBtnBack.onClick.RemoveAllListeners();
    }
    public void PlayGame()
    {
        GameBootstrapper.Instance.SceneManager.GoToGameplay();
        GameBootstrapper.Instance.SceneManager.ActivateScene();
    }
    public void OpenSettings()
    {
        AudioEvents.RaisePlayUI(clickSound);
        settingsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void OpenCredits()
    {
        AudioEvents.RaisePlayUI(clickSound);
        creditsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void QuitGame()
    {
        AudioEvents.RaisePlayUI(clickSound);
        Application.Quit();
    }
    public void CloseSettings()
    {
        AudioEvents.RaisePlayUI(clickSound);
        settingsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void CloseCredits()
    {
        AudioEvents.RaisePlayUI(clickSound);
        creditsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void GoToMain()
    {
        main.SetActive(true);
    }
    private void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        entry.callback.AddListener
            (
                (eventData) => { AudioEvents.RaisePlayUI(hoverSound); }
            );

        trigger.triggers.Add(entry);
    }
}