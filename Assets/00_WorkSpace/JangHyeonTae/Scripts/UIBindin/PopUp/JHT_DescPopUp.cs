using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JHT_DescPopUp : YSJ_PopupBaseUI
{
    private TextMeshProUGUI descText => GetUI<TextMeshProUGUI>("DescText");

    protected override void Start()
    {
        base.Start();
        // Ŀ���� �ʱ�ȭ
        
    }

    public void Init(string desc)
    {
        descText.text = desc;
    }

    public override void Close()
    {
        base.Close();
        // �ݱ� �� ����
        descText.text = "";
    }
}
