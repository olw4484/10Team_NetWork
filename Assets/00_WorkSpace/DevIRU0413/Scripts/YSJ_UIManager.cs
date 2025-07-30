using Runtime.UI;
using Scripts.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_UIManager : YSJ_SimpleSingleton<YSJ_UIManager>
{
    #region Fields

    private readonly Dictionary<YSJ_UIType, Canvas> _canvasMap = new();
    private Dictionary<Canvas, List<JHT_BaseUI>> _uiMap = new();
    private YSJ_PopupController _popupController = new();

    #endregion

    #region Init Methods

    protected override void Init()
    {
        base.Init();
        InitCanvasLayers();
        _popupController.Init();
    }

    private void InitCanvasLayers()
    {
        foreach (YSJ_UIType layer in System.Enum.GetValues(typeof(YSJ_UIType)))
        {
            CreateCanvasIfNotExists(layer);
        }
    }

    #endregion

    #region Canvas Management

    private void CreateCanvasIfNotExists(YSJ_UIType layer)
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

        _uiMap.Add(canvas, new List<JHT_BaseUI>());
        return canvas;
    }

    public Canvas GetCanvas(YSJ_UIType layer)
    {
        if (_canvasMap.TryGetValue(layer, out Canvas canvas))
            return canvas;

        CreateCanvasIfNotExists(layer);
        return _canvasMap[layer];
    }

    #endregion

    #region Clear Methods

    public void TypeClear(YSJ_UIType layer)
    {
        if (layer == YSJ_UIType.Popup)
        {
            var count = _popupController.GetPopupCount();
            _popupController.CloseAll();
#if UNITY_EDITOR
            Debug.Log($"[UIManager] {layer} 레이어의 {count}개 UI 오브젝트가 제거되었습니다.");
#endif
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

    #region Register / UnRegister

    public void RegisterUI(JHT_BaseUI baseUI)
    {
        if (baseUI == null) return;
        if(baseUI.UIType == YSJ_UIType.Popup)
            _popupController.Register(baseUI.gameObject);

        Canvas typeCanvas = GetCanvas(baseUI.UIType);
        baseUI.transform.parent = typeCanvas.transform;

        _uiMap[typeCanvas]?.Add(baseUI);
    }

    public void UnRegisterUI(JHT_BaseUI baseUI)
    {
        if (baseUI == null) return;
        if (baseUI.UIType == YSJ_UIType.Popup)
            _popupController.Unregister(baseUI.gameObject);

        Canvas typeCanvas = GetCanvas(baseUI.UIType);
        _uiMap[typeCanvas]?.Remove(baseUI);
    }

    #endregion

    #region Popup Methods
    public void CloseTopPopup()
    {
        _popupController.CloseTop();
    }

    #endregion
}
