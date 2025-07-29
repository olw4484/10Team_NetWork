using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RedFullPanel : YSJ_PopupBaseUI
{
    protected override void Start()
    {
        base.Start();
        // 커스텀 초기화
        GetEvent("RedBackButton").Click += data =>
        {
            YSJ_UIManager.Instance.ClosePopup(gameObject);
        };
    }

    public override void Close()
    {
        base.Close();
        // 닫기 후 로직
    }


}
