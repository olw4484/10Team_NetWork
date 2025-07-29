using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events :MonoBehaviour
{
    //인벤토리 아이템 추가 제거
    [System.Serializable]
    public class ItemEvent : UnityEvent<SHI_ItemBase> { }
    

    // 인벤토리 슬롯 클릭 시
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    // 단순 실행용 (매개변수 없음)
    [System.Serializable]
    public class VoidEvent : UnityEvent { }

    // 플레이어 경험치 획득 시
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    // UI 상태 갱신 등 다양한 용도로 확장 가능
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
}
