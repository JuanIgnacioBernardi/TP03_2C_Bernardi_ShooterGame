using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    private void Awake ()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject);
        OnAwaken();
    }
    private void OnDestroy ()
    {
        if (Instance == this)
        {
            instance = null;
            OnDestroyed();
        }
    }
    protected virtual void OnAwaken () { }

    protected virtual void OnDestroyed () { }
}