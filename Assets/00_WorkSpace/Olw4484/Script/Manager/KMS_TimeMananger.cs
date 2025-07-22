using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 게임의 경과 시간과 페이즈에 따른 이벤트를 관리하는 TimeManager 싱글턴 클래스.
/// 8분, 15분에 이벤트를 발생시키며, 하드코딩된 타이밍을 기준으로 작동.
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float ElapsedTime { get; private set; }

    public event Action OnPhase1Started; // 게임 시작 0분
    public event Action OnPhase2Started; // 게임 시작 8분
    public event Action OnPhase3Started; // 게임 시작 15분

    private bool _phase2Fired = false;
    private bool _phase3Fired = false;

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

    // 시간에 따른 초기 이벤트 발생 (페이즈1)

    private void Start()
    {
        OnPhase1Started?.Invoke();
    }

    // 시간 변화는 고정 상수로 사용할 예정이므로 시간은 직접 숫자를 입력하여 하드코딩.

    private void Update()
    {
        ElapsedTime += Time.deltaTime;

        if (!_phase2Fired && ElapsedTime >= 480f) // 8분
        {
            _phase2Fired = true;
            OnPhase2Started?.Invoke();
        }

        if (!_phase3Fired && ElapsedTime >= 900f) // 15분
        {
            _phase3Fired = true;
            OnPhase3Started?.Invoke();
        }
    }
}