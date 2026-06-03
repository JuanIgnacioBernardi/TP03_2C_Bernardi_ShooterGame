using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSetting", menuName = "Pool/Setting")]
public class PoolSettingSo : ScriptableObject
{
    public List<PoolSetting> poolSettings = new();
}
[Serializable]
public class PoolSetting
{
    public GameObject prefab;
    public int quantity = 10;
}