using System.Collections.Generic;

using UnityEngine;

public class YSJ_Pool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Stack<GameObject> _poolStack = new Stack<GameObject>();

    private GameObject _group;

    public YSJ_Pool(GameObject prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        _group = new GameObject($"Group [{prefab.name}]");
        _group.transform.parent = parent;

        CreatePool(prefab, initialSize);
    }

    public GameObject Get()
    {
        GameObject obj = _poolStack.Count > 0
                ? _poolStack.Pop()
                : GameObject.Instantiate(_prefab, _parent);

        obj.SetActive(true);
        foreach (var comp in obj.GetComponents<YSJ_IPoolable>())
            comp.OnSpawned();

        return obj;
    }

    public void Release(GameObject obj)
    {
        foreach (var comp in obj.GetComponents<YSJ_IPoolable>())
            comp.OnDespawned();

        obj.SetActive(false);
        _poolStack.Push(obj);
    }

    public void CreatePool(GameObject prefab, int initialSize)
    {
        int index = (_poolStack.Count - 1 <= 0) ? 0 : _poolStack.Count - 1;

        for (int i = index; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, _group.transform);
            obj.SetActive(false);
            _poolStack.Push(obj);
        }
    }

    public void Clear()
    {
        while (_poolStack.Count > 0)
        {
            GameObject obj = _poolStack.Pop();
            GameObject.Destroy(obj);
        }
        _poolStack.Clear();
    }
}
