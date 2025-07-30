using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_TooltipBaseUI : JHT_BaseUI
{
    #region Field
    public override YSJ_UIType UIType => YSJ_UIType.Tooltip;

    #endregion

    #region Unity Method
    private void Start() => Open();

    #endregion
}
