using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RedFullPanel : JHT_BaseUI
{

    private void Start()
    {
        GetEvent("RedBackButton").Click += data =>
        {
            JHT_UIManager.UIInstance.ClosePopUp();
        };
    }
}
