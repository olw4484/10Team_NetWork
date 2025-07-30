using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_BlueFullPanel : YSJ_PopupBaseUI
{
    protected override void Start()
    {
        base.Start();
        // 커스텀 초기화
        GetEvent("BlueBackButton").Click += data =>
        {
            YSJ_UIManager.Instance.UnregisterPopup(gameObject);
        };
    }

    public override void Close()
    {
        base.Close();
        // 닫기 후 로직
    }
}
