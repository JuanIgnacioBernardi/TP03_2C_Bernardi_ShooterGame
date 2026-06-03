// MyPoolManager.cs — versión del profe corregida, adaptada al Bootstrapper
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPoolManager : MonoBehaviour
{
    private PoolSettingSo _settings;  // recibido por Init(), no serializado
    private Dictionary<Type, List<IPooleable>> _pools = new(); // ← inicializado

    public void Init(PoolSettingSo settings)
    {
        _settings = settings;
        SceneManager.activeSceneChanged += OnSceneChanged;
        InitializePool();
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    private void InitializePool()
    {
        foreach (PoolSetting pool in _settings.poolSettings)
        {
            IPooleable pooleable = pool.prefab.GetComponent<IPooleable>();

            if (pooleable == null)
            {
                Debug.LogWarning($"[Pool] {pool.prefab.name} no implementa IPooleable.");
                continue;
            }

            Type type = pooleable.GetType();

            if (_pools.ContainsKey(type))
            {
                Debug.LogWarning($"[Pool] Ya existe pool de {type.Name}.");
                continue;
            }

            _pools[type] = new List<IPooleable>();

            GameObject parent = new(pool.prefab.name);
            parent.transform.SetParent(transform);

            CreatePool(pool.prefab, parent.transform, pool.quantity, _pools[type]);
        }
    }
    public void CreatePool(GameObject prefab, Transform parent, int quantity, List<IPooleable> list)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject go = Instantiate(prefab, parent);
            IPooleable pooleable = go.GetComponent<IPooleable>();
            pooleable.DeActivate();
            list.Add(pooleable);
        }
    }
    public T GetFromPool<T>() where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);
        if (!_pools.ContainsKey(type)) return null;

        foreach (IPooleable item in _pools[type])
            if (!item.IsActive) return item as T;

        return null;
    }
    private void OnSceneChanged(Scene old, Scene next)
    {
        foreach (var list in _pools.Values)
            foreach (IPooleable item in list)
                item.DeActivate();
    }
}