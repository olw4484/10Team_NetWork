using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    // �Ϲ������� ������(�̴Ͼ�, �÷��̾� ��) ��Ʈ�ѷ� ����
    public BaseMinionController minionController;

    // �ִϸ��̼� �̺�Ʈ���� �� �޼��带 ȣ��
    public void OnAttackAnimationEvent()
    {
        if (minionController == null)
        {
            Debug.LogError("[AnimationEventHandler] minionController ������ �����ϴ�.");
            return;
        }
        minionController.OnAttackAnimationEvent();
    }
}