using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerGroup : MonoBehaviour
{
    #region Singleton
    private static ManagerGroup _instance;
    public static ManagerGroup Instance
    {
        get
        {
            if (_instance == null)
            {
                string groupName = $"@{typeof(ManagerGroup).Name}";
                GameObject go = GameObject.Find(groupName);
                if (go == null)
                {
                    go = new GameObject(groupName);
                    DontDestroyOnLoad(go);
                }

                _instance = go.GetOrAddComponent<ManagerGroup>();
            }

            return _instance;
        }
    }
    #endregion

    #region PrivateVariables
    private List<IManager> _unregisteredManagers  = new(); // 미등록
    private List<IManager> _registeredManagers = new(); // 등록됨

    private bool _isManagersInitialized = false; // 초기화 중간 확인 및 매니저들 사용 여부 확인용
    #endregion

    #region PublicMethod

    public bool IsUseAble()
    {
        return _isManagersInitialized;
    }

    public void RegisterManager(IManager manager)
    {
        if (manager == null || _registeredManagers.Contains(manager) || _unregisteredManagers.Contains(manager))
            return;

        foreach (var m in _registeredManagers)
        {
            if (m.Equals(manager))
                return;
        }

        foreach (var m in _unregisteredManagers)
        {
            if (m.Equals(manager))
                return;
        }

        _unregisteredManagers.Add(manager);
    }

    public void RegisterManager(GameObject managerObject)
    {
        RegisterManager(managerObject?.GetComponent<IManager>());
    }

    public void RegisterManager(params IManager[] managers)
    {
        foreach (IManager m in managers) RegisterManager(m);
    }

    public void RegisterManager(params GameObject[] managerObjects)
    {
        foreach (GameObject go in managerObjects) RegisterManager(go);
    }

    public void InitializeManagers()
    {
        _isManagersInitialized = false;
        SortManagersByPriorityAscending(_unregisteredManagers);

        foreach (var manager in _unregisteredManagers)
        {
            manager.Initialize();
            GameObject goM = manager.GetGameObject();
            if (goM == null)
            {
                Debug.LogError($"[Dnot Init] {goM.name} !!!");
                continue;
            }

            Debug.Log($"[Init] {goM.name}");
            _registeredManagers.Add(manager);
            goM.transform.parent = transform;
        }

        _unregisteredManagers.Clear();
        _isManagersInitialized = true;
    }

    /// <summary>
    /// 매니저들 내부 데이터 종료 처리 밎 정리
    /// </summary>
    public void CleanupManagers()
    {
        for (int i = 0; i < _registeredManagers.Count; i++)
        {
            IManager manager = _registeredManagers[i];
            GameObject go = manager.GetGameObject();

            if (go == null)
            {
                _registeredManagers.Remove(manager);
                continue;
            }

            manager.Cleanup();
            Debug.Log($"[Cleanup] {go.name}");
        }
    }

    /// <summary>
    /// 지속적인 생존이 필요하지 않은 매니저 정리
    /// </summary>
    /// <param name="forceClear">강제 정리 여부</param>
    public void ClearManagers(bool forceClear = false)
    {
        for (int i = 0; i < _registeredManagers.Count; i++)
        {
            IManager manager = _registeredManagers[i];
            if (!manager.IsDontDestroy || forceClear)
            {
                GameObject go = manager.GetGameObject();

                if (go == null)
                {
                    _registeredManagers.Remove(manager);
                    continue;
                }

                manager.Cleanup();
                string name = go.name;
                Destroy(go);
                Debug.Log($"[Clear] {name}");
            }
        }
    }

    public void ClearAllManagers()
    {
        ClearManagers(true);
    }
    #endregion

    #region PrivateMethod

    private void SortManagersByPriorityAscending(List<IManager> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                if (list[j].Priority > list[j + 1].Priority)
                {
                    IManager temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    private void SortManagersByPriorityDescending(List<IManager> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                if (list[j].Priority < list[j + 1].Priority)
                {
                    IManager temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    #endregion
}