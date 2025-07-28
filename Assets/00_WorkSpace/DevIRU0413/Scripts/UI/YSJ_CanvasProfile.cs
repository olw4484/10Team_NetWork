using UnityEngine;

[System.Serializable]
public class YSJ_CanvasProfile
{
    public string canvasName;
    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    public int sortingOrder = 0;

    public YSJ_CanvasProfile(string name, RenderMode mode, int order)
    {
        canvasName = name;
        renderMode = mode;
        sortingOrder = order;
    }
}
