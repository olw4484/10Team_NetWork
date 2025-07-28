using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_BlueFullPanel : JHT_BaseUI
{
    private void Start()
    {
        GetEvent("BlueBackButton").Click += data =>
        {
            JHT_UIManager.UIInstance.ClosePopUp();
        };
    }
}
