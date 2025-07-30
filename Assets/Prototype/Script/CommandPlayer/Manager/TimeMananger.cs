using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// ������ ��� �ð��� ����� ���� �̺�Ʈ�� �����ϴ� TimeManager �̱��� Ŭ����.
/// 8��, 15�п� �̺�Ʈ�� �߻���Ű��, �ϵ��ڵ��� Ÿ�̹��� �������� �۵�.
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float ElapsedTime { get; private set; }

    public event Action OnPhase1Started; // ���� ���� 0��
    public event Action OnPhase2Started; // ���� ���� 8��
    public event Action OnPhase3Started; // ���� ���� 15��

    private bool _phase2Fired = false;
    private bool _phase3Fired = false;

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

    // �ð��� ���� �ʱ� �̺�Ʈ �߻� (������1)

    private void Start()
    {
        OnPhase1Started?.Invoke();
    }

    // �ð� ��ȭ�� ���� ����� ����� �����̹Ƿ� �ð��� ���� ���ڸ� �Է��Ͽ� �ϵ��ڵ�.

    private void Update()
    {
        ElapsedTime += Time.deltaTime;

        if (!_phase2Fired && ElapsedTime >= 480f) // 8��
        {
            _phase2Fired = true;
            OnPhase2Started?.Invoke();
        }

        if (!_phase3Fired && ElapsedTime >= 900f) // 15��
        {
            _phase3Fired = true;
            OnPhase3Started?.Invoke();
        }
    }
}