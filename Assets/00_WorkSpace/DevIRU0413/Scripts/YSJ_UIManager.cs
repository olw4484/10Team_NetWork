using Runtime.UI;
using Scripts.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_UIManager : YSJ_SimpleSingleton<YSJ_UIManager>
{
    #region Fields

    private readonly Dictionary<YSJ_UITypes, Canvas> _canvasMap = new();
    private YSJ_PopupController _popupController = new();

    #endregion

    #region Init

    protected override void Init()
    {
        base.Init();
        InitCanvasLayers();
        _popupController.Init();
    }

    private void InitCanvasLayers()
    {
        foreach (YSJ_UITypes layer in System.Enum.GetValues(typeof(YSJ_UITypes)))
        {
            CreateCanvasIfNotExists(layer);
        }
    }

    #endregion

    #region Canvas Management

    private void CreateCanvasIfNotExists(YSJ_UITypes layer)
    {
        if (_canvasMap.ContainsKey(layer)) return;

        string name = $"{layer}_Canvas";
        Canvas canvas = CreateCanvas(new YSJ_CanvasProfile(name, RenderMode.ScreenSpaceOverlay, (int)layer));
        _canvasMap.Add(layer, canvas);
    }

    private Canvas CreateCanvas(YSJ_CanvasProfile profile)
    {
        GameObject go = new GameObject(profile.canvasName);
        go.transform.SetParent(this.transform); // UIManager 자식으로 등록

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = profile.renderMode;
        canvas.sortingOrder = profile.sortingOrder;

        if (profile.renderMode == RenderMode.ScreenSpaceCamera)
            canvas.worldCamera = Camera.main;

        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    public Canvas GetCanvas(YSJ_UITypes layer)
    {
        if (_canvasMap.TryGetValue(layer, out Canvas canvas))
            return canvas;

        CreateCanvasIfNotExists(layer);
        return _canvasMap[layer];
    }

    #endregion

    #region Clear Methods

    public void TypeClear(YSJ_UITypes layer)
    {
        if (layer == YSJ_UITypes.Popup)
        {
            var count = _popupController.GetPopupCount();
#if UNITY_EDITOR
            Debug.Log($"[UIManager] {layer} 레이어의 {count}개 UI 오브젝트가 제거되었습니다.");
#endif
            _popupController.CloseAll();
            return;
        }

        if (!_canvasMap.TryGetValue(layer, out Canvas canvas))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[UIManager] {layer} 레이어에 해당하는 Canvas가 없습니다.");
#endif
            return;
        }

        Transform parent = canvas.transform;
        int childCount = parent.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }

#if UNITY_EDITOR
        Debug.Log($"[UIManager] {layer} 레이어의 {childCount}개 UI 오브젝트가 제거되었습니다.");
#endif
    }

    public void AllClear()
    {
        foreach (var kvp in _canvasMap)
        {
            TypeClear(kvp.Key);
        }
#if UNITY_EDITOR
        Debug.Log("[UIManager] 모든 Canvas 레이어의 UI 오브젝트를 정리했습니다.");
#endif
    }

    #endregion

    #region Popup Methods

    public void RegisterPopup(GameObject popup)
    {
        _popupController.Register(popup);
    }

    public void CloseTopPopup()
    {
        _popupController.CloseTop();
    }

    public void UnregisterPopup(GameObject popup)
    {
        _popupController.Unregister(popup);
    }

    #endregion
}
