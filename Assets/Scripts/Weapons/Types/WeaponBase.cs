using UnityEngine;
using System.Collections;
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] protected WeaponDataSO data;

    protected float damage;
    protected float range;
    protected float fireRate;
    protected float reloadTime;
    protected int magazineSize;

    protected int currentAmmo;
    protected bool isReloading;
    private bool isPaused;
    private float nextFireTime;
    protected bool IsPaused => isPaused;
    private void OnEnable() => GameEvents.onPauseChanged += OnPauseChanged;
    private void OnDisable() => GameEvents.onPauseChanged -= OnPauseChanged;
    private void OnPauseChanged(bool paused) => isPaused = paused;
    protected virtual void Awake()
    {
        if (data != null)
        {
            damage = data.damage;
            range = data.range;
            fireRate = data.fireRate;
            reloadTime = data.reloadTime;
            magazineSize = data.magazineSize;
        }
        currentAmmo = magazineSize;
    }
    public void TryShoot()
    {
        if (isPaused || GameEvents.IsGameOver || isReloading || currentAmmo <= 0 || Time.time < nextFireTime) return;
        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;
        Shoot();
    }
    public void TryReload()
    {
        if (isPaused || GameEvents.IsGameOver || isReloading || currentAmmo == magazineSize) return;
        StartCoroutine(ReloadRoutine());
    }
    protected abstract void Shoot();

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        OnReloadStart();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        OnReloadEnd();
    }
    protected void PlayMuzzleEffects(
    ParticleSystem flash,
    Light light,
    float duration,
    ref Coroutine lightCoroutine)
    {
        if (flash != null)
        {
            flash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            flash.Play();
        }

        if (light != null)
        {
            if (lightCoroutine != null) StopCoroutine(lightCoroutine);
            lightCoroutine = StartCoroutine(MuzzleLightRoutine(light, duration));
        }
    }
    private IEnumerator MuzzleLightRoutine(Light light, float duration)
    {
        light.enabled = true;
        yield return new WaitForSeconds(duration);
        light.enabled = false;
    }
    protected void SpawnImpact(RaycastHit hit)
    {
        if (GameBootstrapper.Instance == null) return;

        ImpactParticle impact = GameBootstrapper.Instance.PoolManager.GetFromPool<ImpactParticle>();
        impact?.Spawn(hit.point, hit.normal);
    }
    protected virtual void OnReloadStart() { }
    protected virtual void OnReloadEnd() { }
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
    public WeaponDataSO Data => data;
}
