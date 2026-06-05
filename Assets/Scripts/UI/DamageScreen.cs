// DamageScreen.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DamageScreen : MonoBehaviour
{
    [SerializeField] private Image bloodImage;
    [SerializeField] private float flashSpeed = 3f;

    private float _targetAlpha;
    private Coroutine _flashCoroutine;
    public void UpdateFromHealth(float current, float max)
    {
        // 0 health = alpha 1, full health = alpha 0
        float healthRatio = current / max;
        _targetAlpha = 1f - healthRatio;
        // Below 30% health, alpha increases more aggressively
        _targetAlpha = healthRatio <= 0.3f
            ? Mathf.Lerp(0.6f, 1f, 1f - (healthRatio / 0.3f))
            : Mathf.Lerp(0f, 0.6f, 1f - healthRatio);

        bloodImage.color = new Color(1f, 1f, 1f, _targetAlpha);
    }
    public void ShowDamage()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        // Instant maximum alpha
        bloodImage.color = new Color(1f, 1f, 1f, 1f);
        // return slowly to the alpha base of current life
        while (bloodImage.color.a > _targetAlpha + 0.01f)
        {
            float newAlpha = Mathf.MoveTowards(bloodImage.color.a, _targetAlpha, flashSpeed * Time.deltaTime);
            bloodImage.color = new Color(1f, 1f, 1f, newAlpha);
            yield return null;
        }
        bloodImage.color = new Color(1f, 1f, 1f, _targetAlpha);
        _flashCoroutine = null;
    }
}