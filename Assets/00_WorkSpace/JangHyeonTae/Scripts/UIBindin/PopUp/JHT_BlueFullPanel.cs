using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_BlueFullPanel : YSJ_PopupBaseUI
{
    public override void Open()
    {
        base.Open();
        // Ŀ���� �ʱ�ȭ
        GetEvent("BlueBackButton").Click += data =>
        {
            Close();
        };
    }

    public override void Close()
    {
        base.Close();
        // �ݱ� �� ����
    }
}
