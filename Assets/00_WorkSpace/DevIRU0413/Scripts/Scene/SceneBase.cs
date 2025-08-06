using System;
using System.Buffers.Text;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    // 현재 씬의 고유 ID (상속 클래스에서 지정)
    public abstract SceneID SceneID { get; }
    
    [Header("Manager")]
    [SerializeField] private GameObject[] _managersInOrder;

    [Header("UI")]
    [SerializeField] private GameObject[] _uiPrefabs;

    [Header("Audio")]
    [SerializeField] private AudioClip _bgm;

    public Action OnInitializeAction;

    protected void Awake()
    {
        InitSystem();
        LoadManagers();
        Initialize();

        YSJ_SystemManager.Instance.ChangeState(SystemStateType.Playing);
    }

    private void InitSystem()
    {
#if UNITY_EDITOR
        Debug.Log($"[SceneBase-InitSystem]: Scene {SceneID} Initialize.");
#endif
        // 시스템 초기화
        ManagerGroup.Instance.RegisterManager(YSJ_SystemManager.Instance);

        // ResourceManager
        // SceneManagerEx
        ManagerGroup.Instance.RegisterManager(YSJ_AudioManager.Instance);
        ManagerGroup.Instance.RegisterManager(YSJ_UIManager.Instance);
        ManagerGroup.Instance.RegisterManager(YSJ_PoolManager.Instance);

        ManagerGroup.Instance.InitializeManagers();
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
        ManagerGroup.Instance.RegisterManager(_managersInOrder);

        // 등록된 매니저들을 초기화
        ManagerGroup.Instance.InitializeManagers();
    }

    // 시스템 매니저 보장되고, 등록하려고 했던 매니저들이 모두 초기화된 후 호출됨
    protected virtual void Initialize()
    {
#if UNITY_EDITOR
        Debug.Log($"[SceneBase-Initialize]: Scene {SceneID} Initialize.");
#endif
        ManagerGroup.Instance.GetManager<YSJ_AudioManager>().PlayBgm(_bgm);
        CreateRegisterUI();

        OnInitializeAction?.Invoke();
    }

    private void CreateRegisterUI()
    {
        if (_uiPrefabs == null || _uiPrefabs.Length <= 0) return;

        var uiManager = ManagerGroup.Instance.GetManager<YSJ_UIManager>();
        foreach (var go in _uiPrefabs)
        {
            var baseUIGo = YSJ_UISpawnFactory.SpawnUI(go);
            var baseUICmp = baseUIGo.GetComponent<JHT_BaseUI>();
            uiManager.RegisterUI(baseUICmp);
        }
    }
}
