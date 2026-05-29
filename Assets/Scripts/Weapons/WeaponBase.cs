using UnityEngine;
using System.Collections;
public abstract class WeaponBase : MonoBehaviour
{
    protected float damage;
    protected float range;
    protected float fireRate;
    protected float reloadTime;
    protected int magazineSize;

    protected int currentAmmo;
    protected bool isReloading;
    private float nextFireTime;
    protected virtual void Awake()
    {
        currentAmmo = magazineSize;
    }
    public void TryShoot()
    {
        if (isReloading || currentAmmo <= 0 || Time.time < nextFireTime) return;
        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;
        Shoot();
    }
    public void TryReload()
    {
        if (isReloading || currentAmmo == magazineSize) return;
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
    protected virtual void OnReloadStart() { }
    protected virtual void OnReloadEnd() { }
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
}
