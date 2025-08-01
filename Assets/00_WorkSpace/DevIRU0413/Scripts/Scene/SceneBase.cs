using System;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    // 현재 씬의 고유 ID (상속 클래스에서 지정)
    public abstract SceneID SceneID { get; }

    [SerializeField] private GameObject[] managersInOrder;

    public Action OnInitializeAction;

    protected void Awake()
    {
        InitSystem();
        LoadManagers();
        Initialize();
        LoadUI();

        YSJ_SystemManager.Instance.ChangeState(SystemStateType.Playing);
    }

    private void InitSystem()
    {
#if UNITY_EDITOR
        Debug.Log($"[SceneBase-InitSystem]: Scene {SceneID} Initialize.");
#endif
        // 시스템 매니저 초기화
        YSJ_SystemManager systemManager = YSJ_SystemManager.Instance;
        ManagerGroup.Instance.RegisterManager(systemManager);
    }

    // 매니저 등록 및 초기화
    private void LoadManagers()
    {
#if UNITY_EDITOR
        Debug.Log($"[SceneBase-LoadManagers]: Scene {SceneID} Initialize.");
#endif
        // 현재 씬에 존재하는 Manager 태그 오브젝트 수집
        // GameObject[] SubscribeManagers = GameObject.FindGameObjectsWithTag("Manager");

        // 기존에 불필요한 매니저 제거
        ManagerGroup.Instance.ClearManagers();

        // 남아 있는 매니저 클린업 (종료 처리 등)
        ManagerGroup.Instance.CleanupManagers();

        // 새로 찾은 매니저 오브젝트들을 등록
        // ManagerGroup.Instance.RegisterManager(SubscribeManagers);
        ManagerGroup.Instance.RegisterManager(managersInOrder);

        // 등록된 매니저들을 초기화
        ManagerGroup.Instance.InitializeManagers();
    }

    protected virtual void Initialize()
    {
#if UNITY_EDITOR
        Debug.Log($"[SceneBase-Initialize]: Scene {SceneID} Initialize.");
#endif
        OnInitializeAction?.Invoke();
    }

    private void LoadUI()
    {
        // UI 프리팹을 순회하며 UI 매니저에 설정

        // 현재 씬에 존재하는 BaseUI 오브젝트 수집
        // GameObject[] SubscribeManagers = GameObject.FindGameObjectsWithTag("Manager");
    }
}
