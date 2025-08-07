using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public BaseMinionController minionController;

    // Animation Event에서 호출됨 (예: Attack 애니 프레임)
    public void OnAttackAnimationEvent()
    {
        if (minionController == null)
        {
            Debug.LogError("[AnimationEventHandler] minionController 미지정");
            return;
        }
        minionController.OnAttackAnimationEvent();
    }
}