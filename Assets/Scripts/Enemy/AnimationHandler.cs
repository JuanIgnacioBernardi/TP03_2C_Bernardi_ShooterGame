using UnityEngine;
public class AnimationHandler : MonoBehaviour
{
    private EnemyShoot _enemyShoot;
    private void Awake()
    {
        // Finding EnemyShoot component in the parent GameObject
        _enemyShoot = GetComponentInParent<EnemyShoot>();
    }

    // Call this event from the animation that has the event
    public void ThrowGrenade()
    {
        _enemyShoot?.ThrowGrenade();
    }
}
