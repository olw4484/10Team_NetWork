using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    // 일반적으로 공격자(미니언, 플레이어 등) 컨트롤러 참조
    public BaseMinionController minionController;

    // 애니메이션 이벤트에서 이 메서드를 호출
    public void OnAttackAnimationEvent()
    {
        if (minionController == null)
        {
            Debug.LogError("[AnimationEventHandler] minionController 참조가 없습니다.");
            return;
        }
        minionController.OnAttackAnimationEvent();
    }
}