using UnityEngine;

[System.Serializable]
public class YSJ_CanvasProfile
{
    [field: SerializeField] public GameObject canvasCameraPrefab { get; private set; } = null;

    public YSJ_UIType canvasType;
    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    public int customSortingOrder = -1;

    public YSJ_CanvasProfile(YSJ_UIType uiType, RenderMode mode, int order = -1)
    {
        canvasType = uiType;
        renderMode = mode;
        customSortingOrder = order;
    }
}
