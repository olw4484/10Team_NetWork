using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_EventManager : MonoBehaviour
{
    public static KMS_EventManager Instance { get; private set; }

    // �̱��� �ʱ�ȭ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Minion�� ����� ��

    public event Action<GameObject, KMS_MinionController> OnMinionKillConfirmed;
    public void MinionKillConfirmed(GameObject killer, KMS_MinionController victim)
    {
        OnMinionKillConfirmed?.Invoke(killer, victim);
    }

    // Minion�� �׾��� ��

    public event Action<KMS_MinionController, GameObject> OnMinionDead;
    public void MinionDead(KMS_MinionController victim, GameObject killer)
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
}
