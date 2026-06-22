using System.Collections;
using UnityEngine;
using TMPro;

public class IntroHints : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Messages")]
    [SerializeField]
    private string[] messages = new string[]
    {
        "Elimina los 4 campamentos enemigos para ganar",
        "Presioná 1 / 2 o Scroll para cambiar de arma",
        "Presioná 'Q' para usar un botiquín"
    };
    [Header("Timing")]
    [SerializeField] private float fadeInTime = 0.4f;
    [SerializeField] private float holdTime = 2.5f;
    [SerializeField] private float fadeOutTime = 0.6f;
    [SerializeField] private float gapBetweenMessages = 0.3f;
    private void Start()
    {
        SetAlpha(0f);
        StartCoroutine(PlaySequence());
    }
    private IEnumerator PlaySequence()
    {
        foreach (string msg in messages)
        {
            hintText.text = msg;
            yield return Fade(0f, 1f, fadeInTime);
            yield return new WaitForSeconds(holdTime);
            yield return Fade(1f, 0f, fadeOutTime);
            yield return new WaitForSeconds(gapBetweenMessages);
        }
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
        Color c = hintText.color;
        c.a = alpha;
        hintText.color = c;
    }
}