using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // 싱글턴 초기화

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
}
