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

    public void Init(string desc,Vector2 vectorSet)
    {
        descText.text = desc; 
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.position = vectorSet;
        }
    }

    public override void Close()
    {
        base.Close();
        // �ݱ� �� ����
        descText.text = "";
    }
}
