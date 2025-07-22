using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

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

    public event Action<MinionController, GameObject> OnMinionKilled;

    public void MinionKilled(MinionController victim, GameObject killer)
    {
        OnMinionKilled?.Invoke(victim, killer);
    }
}
