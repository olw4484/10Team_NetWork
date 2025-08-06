using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_SystemBaseUI : JHT_BaseUI
{
    #region Field
    public override YSJ_UIType UIType => YSJ_UIType.System;

    #endregion

    #region Unity Method
    private void Start() => Open();

    #endregion
}