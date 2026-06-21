using System.Collections;
using UnityEngine;
using TMPro;

public class CampClearedNotification : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Timing")]
    [SerializeField] private float fadeInTime = 0.4f;
    [SerializeField] private float holdTime = 1.5f;
    [SerializeField] private float fadeOutTime = 0.6f;

    [Header("Audio")]
    [SerializeField] private AudioClip clearedSound;

    private Coroutine activeRoutine;

    private void OnEnable() => GameEvents.onCampCleared += OnCampCleared;
    private void OnDisable() => GameEvents.onCampCleared -= OnCampCleared;

    private void Start()
    {
        SetAlpha(0f);
    }
    private void OnCampCleared(CampController camp)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ShowRoutine());
    }
    private IEnumerator ShowRoutine()
    {
        text.text = "¡Campamento liberado!";

        if (clearedSound != null)
            AudioEvents.RaisePlaySFX(clearedSound);

        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);

        activeRoutine = null;
    }
    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
    }
    private void SetAlpha(float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}