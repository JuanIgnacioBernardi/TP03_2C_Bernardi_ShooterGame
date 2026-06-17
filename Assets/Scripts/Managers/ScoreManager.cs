using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerCamp = 1000;

    private int score;
    private void OnEnable() => GameEvents.onCampCleared += OnCampCleared;
    private void OnDisable() => GameEvents.onCampCleared -= OnCampCleared;

    private void Start() => UpdateUI();
    private void OnCampCleared(CampController camp)
    {
        score += pointsPerCamp;
        UpdateUI();
    }
    private void UpdateUI() => scoreText.text = $"Score: {score}";
}