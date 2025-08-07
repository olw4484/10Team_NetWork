using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public BaseMinionController minionController;

    // Animation Event���� ȣ��� (��: Attack �ִ� ������)
    public void OnAttackAnimationEvent()
    {
        if (minionController == null)
        {
            Debug.LogError("[AnimationEventHandler] minionController ������");
            return;
        }
        minionController.OnAttackAnimationEvent();
    }
}