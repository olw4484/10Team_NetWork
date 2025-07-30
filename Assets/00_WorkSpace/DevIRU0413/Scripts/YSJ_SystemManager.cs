using Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ���߿� �ڸ� �ű� �� ������ �ű� ����
public enum SystemStateType
{
    Init,
    SceneChange,
    Playing,
    Paused,
    Quit,
}

public class YSJ_SystemManager : YSJ_SimpleSingleton<YSJ_SystemManager>
{
    #region Field
    public ILoadingUI LoadingUI;

    public SystemStateType CurrentState { get; private set; }
    #endregion

    #region SimpleSingleton Override Method

    protected override void Init()
    {
        base.Init();

        ChangeState(SystemStateType.Init);
        EventManager.Instance.OnHQDestroyed += HandleHQDestroyed;
    }

    protected override void Destroy()
    {
        base.Destroy();
        if (EventManager.Instance != null)
            EventManager.Instance.OnHQDestroyed -= HandleHQDestroyed;
    }

    #endregion

    #region Unity Method

    private void Update()
    {
        if (_states != null)
            _states[CurrentState]?.Tick();
    }

    #endregion

    #region Handle
    // Handle
    private void HandleHQDestroyed(int teamId)
    {
        BroadcastGameOver(teamId);
    }
    #endregion

    // State
    public void ChangeState(in SystemStateType newStateType)
    {
        if (CurrentState == newStateType) return;

        CurrentState = newStateType;
        Debug.Log($"Game State Changed: {CurrentState}");
    }
    private void BroadcastGameOver(int losingTeamId)
    {
        ChangeState(SystemStateType.Quit);
        Debug.Log($"Game Over! �� {losingTeamId} �й�");
    }


    #region �� �ε�
    // �� �Ŵ��� ���� ����

    public void LoadSceneWithPreActions(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "�� �ε� ��...")
    {
        StartCoroutine(TransitionScene(sceneName, preActions, finalMessage));
    }

    private IEnumerator TransitionScene(string sceneName, List<(string message, Func<IEnumerator> action)> preActions = null, string finalMessage = "�� �ε� ��...")
    {
        ChangeState(SystemStateType.SceneChange);
        LoadingUI?.Show("�ʱ�ȭ ��..."); // ���߿� ���� ���� ��

        // ��ó�� �׼� ����
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

        // �� �ε�
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }

        LoadingUI?.Hide(); // ���߿� ���� ���� ��
        ChangeState(SystemStateType.Playing);
    }

    #endregion

    #region ���� ����

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

    #region ���� ����/�ҷ�����

    // ����(���� ����)
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
