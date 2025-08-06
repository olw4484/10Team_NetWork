using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events :MonoBehaviour
{
    //�κ��丮 ������ �߰� ����
    [System.Serializable]
    public class ItemEvent : UnityEvent<SHI_ItemBase> { }
    

    // �κ��丮 ���� Ŭ�� ��
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    // �ܼ� ����� (�Ű����� ����)
    [System.Serializable]
    public class VoidEvent : UnityEvent { }

    // �÷��̾� ����ġ ȹ�� ��
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    // UI ���� ���� �� �پ��� �뵵�� Ȯ�� ����
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
}
