using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("Camps")]
    [SerializeField] private CampController[] camps;

    [Header("Player")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Transform playerTransform;

    [Header("UI")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject deathScreen;

    private int clearedCamps;
    private void Start()
    {
        foreach (CampController camp in camps)
        {
            camp.onCampCleared += OnCampCleared;
            camp.Initialize(playerTransform);
        }
        playerHealth.onDie += OnPlayerDied;

        winScreen.SetActive(false);
        deathScreen.SetActive(false);
    }
    private void OnDestroy()
    {
        foreach (CampController camp in camps)
            camp.onCampCleared -= OnCampCleared;

        if (playerHealth != null)
            playerHealth.onDie -= OnPlayerDied;

        Time.timeScale = 1f;
    }
    private void OnCampCleared(CampController camp)
    {
        clearedCamps++;
        Debug.Log($"[GameManager] Campamentos completados: {clearedCamps}/{camps.Length}");

        if (clearedCamps >= camps.Length)
        {
            Debug.Log("[GameManager] VICTORIA");
            ShowWinScreen();
        }
    }
    private void OnPlayerDied() => ShowDeathScreen();
    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    private void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}