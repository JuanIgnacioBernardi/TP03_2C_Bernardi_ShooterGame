using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] AudioClip hoverUI;

    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] Button resumeBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button homeBtn;
    [SerializeField] Button settingsBackBtn;

    public bool isPaused = false;

    private void Awake()
    {
        resumeBtn.onClick.AddListener(ResumeGame);
        settingsBtn.onClick.AddListener(OnSettingsClicked);
        homeBtn.onClick.AddListener(OnHomeClicked);
        settingsBackBtn.onClick.AddListener(OnSettingsBack);

        AddHoverSound(resumeBtn);
        AddHoverSound(settingsBtn);
        AddHoverSound(homeBtn);
        AddHoverSound(settingsBackBtn);
    }
    private void OnDestroy()
    {
        resumeBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        settingsBackBtn.onClick.RemoveAllListeners();
    }
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                OnPauseClicked();
        }
    }
    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        GameEvents.RaisePauseChanged(false);
    }
    private void OnPauseClicked()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        GameEvents.RaisePauseChanged(true);
    }
    private void OnSettingsClicked()
    {
        settingsMenu.SetActive(true);
    }
    private void OnHomeClicked()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    private void OnSettingsBack()
    {
        settingsMenu.SetActive(false);
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
                (eventData) => { AudioEvents.RaisePlayUI(hoverUI); }
            );
        trigger.triggers.Add(entry);
    }
}