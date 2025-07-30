public class YSJ_HUDBaseUI : JHT_BaseUI
{
    #region Field
    public override YSJ_UIType UIType => YSJ_UIType.HUD;

    #endregion

    #region Unity Method
    private void Start() => Open();

    #endregion
}