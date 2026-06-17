using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("Camps")]
    [SerializeField] private CampController[] camps;

    [Header("Player")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Transform playerTransform;

    private int clearedCamps;
    private void Start()
    {
        foreach (CampController camp in camps)
        {
            camp.onCampCleared += OnCampCleared;
            camp.Initialize(playerTransform);
        }
        playerHealth.onDie += OnPlayerDied;
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
            GameEvents.RaiseWin();
        }
    }
    private void OnPlayerDied() => GameEvents.RaiseDeath();
}