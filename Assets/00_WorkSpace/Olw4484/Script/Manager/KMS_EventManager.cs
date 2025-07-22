using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

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
}
