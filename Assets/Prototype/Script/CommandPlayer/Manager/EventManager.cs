using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour, IManager
{
    public static EventManager Instance { get; private set; }

    public int Priority => (int)ManagerPriority.EventManager;

    public bool IsDontDestroy => false;

    // 싱글턴 초기화 / IManager의 Initialize를 사용하므로 주석처리

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //
    //    Instance = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    // Minion을 잡았을 때

    public event Action<GameObject, MinionController> OnMinionKillConfirmed;
    public void MinionKillConfirmed(GameObject killer, MinionController victim)
    {
        OnMinionKillConfirmed?.Invoke(killer, victim);
    }

    // Minion이 죽었을 때

    public event Action<MinionController, GameObject> OnMinionDead;
    public void MinionDead(MinionController victim, GameObject killer)
    {
        OnMinionDead?.Invoke(victim, killer);
    }

    // HQ가 파괴되었을 때 - 팀ID를 넘겨서 패배처리

    public event Action<int> OnHQDestroyed;
    public void HQDestroyed(int destroyedTeamId)
    {
        OnHQDestroyed?.Invoke(destroyedTeamId);
    }

    // 자원의 변화가 있을 때 

    public event Action<int, int> OnResourceChanged;
    public void ResourceChanged(int teamId, int currentGold)
    {
        OnResourceChanged?.Invoke(teamId, currentGold);
    }

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (IsDontDestroy)
            DontDestroyOnLoad(gameObject);
    }

    public void Cleanup()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public GameObject GetGameObject() => this.gameObject;
}
