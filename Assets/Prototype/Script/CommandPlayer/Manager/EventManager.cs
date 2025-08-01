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

    // �̱��� �ʱ�ȭ / IManager�� Initialize�� ����ϹǷ� �ּ�ó��

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

    // Minion�� ����� ��

    public event Action<GameObject, MinionController> OnMinionKillConfirmed;
    public void MinionKillConfirmed(GameObject killer, MinionController victim)
    {
        OnMinionKillConfirmed?.Invoke(killer, victim);
    }

    // Minion�� �׾��� ��

    public event Action<MinionController, GameObject> OnMinionDead;
    public void MinionDead(MinionController victim, GameObject killer)
    {
        OnMinionDead?.Invoke(victim, killer);
    }

    // HQ�� �ı��Ǿ��� �� - ��ID�� �Ѱܼ� �й�ó��

    public event Action<int> OnHQDestroyed;
    public void HQDestroyed(int destroyedTeamId)
    {
        OnHQDestroyed?.Invoke(destroyedTeamId);
    }

    // �ڿ��� ��ȭ�� ���� �� 

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
