using Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 나중에 자리 옮길 수 있으면 옮길 예정
public enum GameState
{
    Init,
    Loading,
    Playing,
    Paused,
    GameOver
}

public class YSJ_GameManager : YSJ_SimpleSingleton<YSJ_GameManager>
{

    #region Public Field
    public ILoadingUI LoadingUI;

    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnGameStateChanged;
    #endregion

    protected override void Init()
    {
        base.Init();
        ChangeState(GameState.Init);
        EventManager.Instance.OnHQDestroyed += HandleHQDestroyed;
    }

    protected override void Destroy()
    {
        base.Destroy();
        if (EventManager.Instance != null)
            EventManager.Instance.OnHQDestroyed -= HandleHQDestroyed;
    }

    #region Handle
    // Handle
    private void HandleHQDestroyed(int teamId)
    {
        BroadcastGameOver(teamId);
    }
    #endregion

    // State
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"Game State Changed: {CurrentState}");
        OnGameStateChanged?.Invoke(CurrentState);
    }

    private void BroadcastGameOver(int losingTeamId)
    {
        ChangeState(GameState.GameOver);
        Debug.Log($"Game Over! 팀 {losingTeamId} 패배");
    }


    #region 씬 로딩
    // 씬 매니저 이전 예상

    public void LoadSceneWithPreActions(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "씬 로딩 중...")
    {
        StartCoroutine(TransitionScene(sceneName, preActions, finalMessage));
    }

    private IEnumerator TransitionScene(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "씬 로딩 중...")
    {
        ChangeState(GameState.Loading);
        LoadingUI?.Show("초기화 중..."); // 나중에 따로 빼도 됨

        // 전처리 액션 실행
        if (preActions != null)
        {
            foreach (var (message, action) in preActions)
            {
                if (!string.IsNullOrEmpty(message))
                    LoadingUI?.UpdateMessage(message);

                if (action != null)
                    yield return StartCoroutine(action());
            }
        }

        LoadingUI?.UpdateMessage(finalMessage);

        // 씬 로딩
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }

        LoadingUI?.Hide(); // 나중에 따로 빼도 됨
        ChangeState(GameState.Playing);
    }

    #endregion

    #region 게임 제어

    public void PauseGame()
    {
        Time.timeScale = 0f;
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        ChangeState(GameState.Playing);
    }

    public void QuitGame()
    {
        Application.Quit();
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
