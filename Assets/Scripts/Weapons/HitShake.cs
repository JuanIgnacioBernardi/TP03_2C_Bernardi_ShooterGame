using System.Collections;
using UnityEngine;
public class HitShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float magnitude = 0.05f;

    private Vector3 originPos;
    private void Awake() => originPos = transform.localPosition;
    public void Shake() => StartCoroutine(ShakeRoutine());
    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = originPos + Random.insideUnitSphere * Mathf.Lerp(magnitude, 0f, elapsed / duration);
            yield return null;
        }
        transform.localPosition = originPos;
    }
}