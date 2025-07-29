using Runtime.UI;
using UnityEngine;

public class YSJ_UISpawnFactory
{
    public static GameObject SpawnUI(GameObject prefab, YSJ_UITypes layer, int extraOrder = 0)
    {
        Canvas parentCanvas = YSJ_UIManager.Instance.GetCanvas(layer);
        if (parentCanvas == null)
        {
            Debug.LogError($"[UISpawnFactory] {layer}_Canvas가 UIManager에 설정되어 있지 않습니다.");
            return null;
        }

        GameObject ui = GameObject.Instantiate(prefab, parentCanvas.transform);

        // 선택적으로 sorting order 세팅
        Canvas overrideCanvas = ui.GetComponent<Canvas>();
        int sortingOrder = (int)layer + extraOrder;
        if (overrideCanvas != null)
        {
            overrideCanvas.overrideSorting = true;
            overrideCanvas.sortingOrder = sortingOrder;
        }

        ui.transform.SetAsLastSibling();
        return ui;
    }

    public static GameObject SpawnUI(string resourcePath, YSJ_UITypes layer, int extraOrder = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        if (prefab == null)
        {
            Debug.LogError($"[UISpawnFactory] Prefab '{resourcePath}' not found!");
            return null;
        }

        return SpawnUI(prefab, layer, extraOrder);
    }

    public static GameObject ShowPopup(GameObject popupPrefab)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("ShowPopup: popupPrefab이 비어있음");
            return null;
        }

        var popup = SpawnUI(popupPrefab, YSJ_UITypes.Popup);

        // RectTransform 위치 초기화 (풀스크린 중앙)
        var rect = popup.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        return popup;
    }
}
