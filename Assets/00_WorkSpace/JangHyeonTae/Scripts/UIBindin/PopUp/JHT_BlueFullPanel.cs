using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_BlueFullPanel : YSJ_PopupBaseUI
{
    public override void Open()
    {
        base.Open();
        // 커스텀 초기화
        GetEvent("BlueBackButton").Click += data =>
        {
            Close();
        };
    }

    public override void Close()
    {
        base.Close();
        // 닫기 후 로직
    }
}
