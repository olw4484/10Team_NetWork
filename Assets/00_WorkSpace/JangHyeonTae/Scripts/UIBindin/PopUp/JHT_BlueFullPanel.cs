using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_BlueFullPanel : YSJ_PopupBaseUI
{
    protected override void Start()
    {
        base.Start();
        // Ŀ���� �ʱ�ȭ
        GetEvent("BlueBackButton").Click += data =>
        {
            YSJ_UIManager.Instance.UnregisterPopup(gameObject);
        };
    }

    public override void Close()
    {
        base.Close();
        // �ݱ� �� ����
    }
}
