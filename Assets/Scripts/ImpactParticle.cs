using System.Collections;
using UnityEngine;
public class ImpactParticle : MonoBehaviour, IPooleable
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float lifeTime = 1f;

    private Coroutine _returnCoroutine;
    public bool IsActive => gameObject.activeSelf;
    public void Activate()
    {
        gameObject.SetActive(true);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();

        if (_returnCoroutine != null) StopCoroutine(_returnCoroutine);
        _returnCoroutine = StartCoroutine(ReturnAfterLifetime());
    }
    public void DeActivate()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
    }
    public void Spawn(Vector3 position, Vector3 normal)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(normal));
        Activate();
    }
    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifeTime);
        DeActivate();
    }
}