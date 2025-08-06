using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SystemStateType
{
    Init,
    SceneChange,
    Playing,
    Paused,
    Quit,
}

public class YSJ_SystemManager : YSJ_SimpleSingleton<YSJ_SystemManager>, IManager
{
    private bool _isInitialized = false;

    #region Field
    [SerializeField] public GameObject _loadingUIPrefab;
    private ILoadingUI _iLoadingUI;

    public SceneBase CurrentSceneBase { get; private set; }
    public SceneID CurrentSceneID { get; private set; }

    public SystemStateType CurrentState { get; private set; }

    public bool IsDontDestroy => isDontDestroyOnLoad;
    #endregion

    #region IManager
    public void Initialize()
    {
        if (_isInitialized) return;

        SceneLoaded();

        CurrentSceneBase.OnInitializeAction -= InitLoadingUI;
        CurrentSceneBase.OnInitializeAction += InitLoadingUI;

        CurrentSceneBase.OnInitializeAction -= TestSceneLoad;
        CurrentSceneBase.OnInitializeAction += TestSceneLoad;

        ChangeState(SystemStateType.Init);

        _isInitialized = true;
    }
    public void Cleanup() { }
    public GameObject GetGameObject() => this.gameObject;

    #endregion

    public void ChangeState(in SystemStateType newStateType)
    {
        if (CurrentState == newStateType) return;

        CurrentState = newStateType;
        Debug.Log($"Game State Changed: {CurrentState}");
    }

    #region 씬 로딩
    // 씬 매니저 이전 예상

    public void LoadSceneWithPreActions(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "Scene Loading...")
    {
        StartCoroutine(TransitionScene(sceneName, preActions, finalMessage));
    }
    public void LoadPhotonSceneWithPreActions(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "Scene Loading...")
    {
        StartCoroutine(TransitionPhotonScene(sceneName, preActions, finalMessage));
    }

    private IEnumerator TransitionScene(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = null)
    {
        ChangeState(SystemStateType.SceneChange);
        _iLoadingUI?.SetProgress(0.0f);
        _iLoadingUI?.Show("Wait Init...");
        yield return new WaitForSeconds(1.0f);

        // 전처리 액션 실행
        if (preActions != null)
        {
            foreach (var (message, action) in preActions)
            {
                if (!string.IsNullOrEmpty(message))
                    _iLoadingUI?.UpdateMessage(message);

                if (action != null)
                    yield return StartCoroutine(action());
            }
        }

        _iLoadingUI?.SetProgress(0.5f);
        _iLoadingUI?.UpdateMessage(finalMessage);
        yield return new WaitForSeconds(1.0f);

        // 씬 로딩
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }

        _iLoadingUI?.SetProgress(1.0f);
        _iLoadingUI?.UpdateMessage("Loaded!!");
        yield return new WaitForSeconds(1.0f);

        _iLoadingUI?.Hide(); // 나중에 따로 빼도 됨
        SceneLoaded();
    }
    private IEnumerator TransitionPhotonScene(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = null)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Photon에 연결되어 있지 않습니다.");
            yield break;
        }

        ChangeState(SystemStateType.SceneChange);
        _iLoadingUI?.SetProgress(0.0f);
        _iLoadingUI?.Show("Wait Init...");
        yield return new WaitForSeconds(1.0f);

        // 전처리 액션
        if (preActions != null)
        {
            foreach (var (message, action) in preActions)
            {
                if (!string.IsNullOrEmpty(message))
                    _iLoadingUI?.UpdateMessage(message);

                if (action != null)
                    yield return StartCoroutine(action());
            }
        }

        _iLoadingUI?.SetProgress(0.5f);
        _iLoadingUI?.UpdateMessage(finalMessage);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(sceneName); // 모든 플레이어에게 전파됨
        }
        else
        {
            // 비마스터는 수동으로 로딩 메시지만 보여줌
            _iLoadingUI?.UpdateMessage("마스터 클라이언트가 씬을 전환 중입니다...");
        }

        _iLoadingUI?.SetProgress(1.0f);
        _iLoadingUI?.UpdateMessage("Loaded!!");
        yield return new WaitForSeconds(1.0f);

        _iLoadingUI?.Hide();
        SceneLoaded();
    }

    private void SceneLoaded()
    {
        GameObject sceneBaseGO = GameObject.FindGameObjectWithTag("SceneBase");
        SceneBase sceneBaseCmp = sceneBaseGO?.GetComponent<SceneBase>();
        if (sceneBaseCmp == null)
        {
            Debug.LogError("SceneBase 컴포넌트가 존재하지 않습니다.");
            return;
        }

        CurrentSceneBase = sceneBaseCmp;
        CurrentSceneID = sceneBaseCmp.SceneID;
    }

    private void InitLoadingUI()
    {
        if (_loadingUIPrefab == null || _iLoadingUI != null) return;

        var loadingUIGO = YSJ_UISpawnFactory.SpawnUI(_loadingUIPrefab);
        _iLoadingUI = loadingUIGO.GetComponent<ILoadingUI>();
        if (_iLoadingUI == null) return;
        _iLoadingUI.Init();
        // _iLoadingUI.Hide();
    }

    private void TestSceneLoad()
    {
        LoadSceneWithPreActions("YSJ_TestScene 1");
    }

    #endregion

    #region 게임 제어

    public void PauseGame()
    {
        ChangeState(SystemStateType.Paused);
    }

    public void ResumeGame()
    {
        ChangeState(SystemStateType.Playing);
    }

    public void QuitGame()
    {
        ChangeState(SystemStateType.Quit);
    }

    #endregion

    #region 설정 저장/불러오기

    // 사운드(개인 설정)
    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
    }
    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("Volume", 0.5f);
    }


    #endregion
}
