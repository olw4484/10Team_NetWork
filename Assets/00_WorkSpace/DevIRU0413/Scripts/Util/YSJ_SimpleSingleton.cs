﻿using UnityEngine;

public abstract class YSJ_SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    [SerializeField, HideInInspector] protected bool isDontDestroyOnLoad = true;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[SimpleSingleton] {typeof(T)} 인스턴스가 없어 에디터에서 자동 생성됨.");
#endif
                    GameObject go = new GameObject($"@SimpleSingleton_{typeof(T)}");
                    _instance = go.AddComponent<T>();

                    if ((_instance as YSJ_SimpleSingleton<T>).isDontDestroyOnLoad)
                        DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake() => Init();
    private void OnDestroy() => Destroy();

    private void Init()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Destroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
