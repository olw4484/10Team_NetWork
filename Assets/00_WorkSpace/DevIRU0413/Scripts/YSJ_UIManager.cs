using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class YSJ_UIManager : YSJ_SimpleSingleton<YSJ_UIManager>, IManager
{
    #region Fields
    [SerializeField] private YSJ_CanvasProfile[] _customCanvasProfiles = new YSJ_CanvasProfile[5];

    private readonly Dictionary<YSJ_UIType, Canvas> _canvasMap = new();
    private Dictionary<Canvas, List<JHT_BaseUI>> _uiMap = new();
    private YSJ_PopupController _popupController = new();

    public bool IsDontDestroy => isDontDestroyOnLoad;
    #endregion

    #region IManager
    public void Initialize()
    {
        InitCanvasLayers();
        _popupController.Init();
    }

    public void Cleanup() { AllClear(YSJ_UIType.System); }
    public GameObject GetGameObject() => this.gameObject;
    #endregion

    #region Init Methods
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

        Canvas canvas = null;
        // 커스텀 컴버스가 있다면
        if (_customCanvasProfiles.Length > 0)
        {
            foreach (var profile in _customCanvasProfiles)
            {
                if (profile == null) continue;
                if (profile.canvasType == layer)
                {
                    canvas = CreateCanvas(profile);
                    break;
                }
            }
        }

        if (canvas == null)
            canvas = CreateCanvas(new YSJ_CanvasProfile(layer, RenderMode.ScreenSpaceOverlay, (int)layer));

        _canvasMap.Add(layer, canvas);
    }

    private Canvas CreateCanvas(YSJ_CanvasProfile profile)
    {
        // 캔버스용 GameObject 생성 및 UIManager 자식으로 설정
        string nameGo = $"{profile.canvasType}_Canvas";
        GameObject go = new GameObject(nameGo);
        go.transform.SetParent(this.transform);

        // 카메라 프리팹이 있을 경우, 인스턴스 생성 및 Camera 컴포넌트 획득
        Camera camera = null;
        if (profile.canvasCameraPrefab != null)
        {
            GameObject goCamera = Instantiate(profile.canvasCameraPrefab, this.transform);
            camera = goCamera.GetComponent<Camera>();
        }

        // Canvas 설정
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = profile.renderMode;
        canvas.sortingOrder = (profile.customSortingOrder == -1) ? (int)profile.canvasType : profile.customSortingOrder;
        canvas.vertexColorAlwaysGammaSpace = true;

        // ScreenSpaceCamera 모드일 경우 worldCamera 설정
        if (profile.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = camera ?? Camera.main;
        }

        // Canvas에 필요한 컴포넌트들 추가
        CanvasScaler scaler = go.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        go.GetOrAddComponent<GraphicRaycaster>();

        // 내부 UI 매핑 리스트 초기화
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

    public void AllClear(params YSJ_UIType[] excludedLayers)
    {
        foreach (var kvp in _canvasMap)
        {
            if (excludedLayers.Length > 0 && !System.Array.Exists(excludedLayers, l => l != kvp.Key))
                continue;
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
        if (baseUI == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[UIManager-RegisterUI]: Not baseUI");
#endif
            return;
        }

        Canvas typeCanvas = GetCanvas(baseUI.UIType);
        if (_uiMap[typeCanvas].Contains(baseUI))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[UIManager-RegisterUI]: is Contains {baseUI}.");
#endif
            return;
        }

        if (baseUI.UIType == YSJ_UIType.Popup)
            _popupController.Register(baseUI.gameObject);

        baseUI.transform.SetParent(typeCanvas.transform, false);
        _uiMap[typeCanvas]?.Add(baseUI);
    }


    public void UnRegisterUI(JHT_BaseUI baseUI)
    {
        if (baseUI == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[UIManager-UnRegisterUI]: Not baseUI");
#endif
            return;
        }

        if (baseUI.UIType == YSJ_UIType.Popup)
            _popupController.Unregister(baseUI.gameObject);

        if (!_canvasMap.TryGetValue(baseUI.UIType, out Canvas canvas))
            return;

        _uiMap[canvas]?.Remove(baseUI);
    }


    #endregion

    #region Popup Methods
    public void CloseTopPopup()
    {
        _popupController.CloseTop();
    }

    #endregion
}
