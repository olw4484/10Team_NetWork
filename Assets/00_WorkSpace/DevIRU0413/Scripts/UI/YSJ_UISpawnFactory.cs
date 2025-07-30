using Runtime.UI;
using Unity.VisualScripting;
using UnityEngine;

public class YSJ_UISpawnFactory
{
    // spawn for prefab
    public static GameObject SpawnUI(GameObject prefab, YSJ_UIType layer, int extraOrder = 0)
    {
        Canvas parentCanvas = YSJ_UIManager.Instance.GetCanvas(layer);
        if (parentCanvas == null)
        {
            Debug.LogError($"[UISpawnFactory] {layer}_Canvas�� UIManager�� �����Ǿ� ���� �ʽ��ϴ�.");
            return null;
        }

        GameObject uiObject = GameObject.Instantiate(prefab);
        JHT_BaseUI uiComp = uiObject.GetOrAddComponent<JHT_BaseUI>();

        return uiObject;
    }

    // spawn for resource
    public static GameObject SpawnUI(string resourcePath, YSJ_UIType layer, int extraOrder = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        if (prefab == null)
        {
            Debug.LogError($"[UISpawnFactory] Prefab '{resourcePath}' not found!");
            return null;
        }

        return SpawnUI(prefab, layer, extraOrder);
    }

    // spawn for prefab
    public static GameObject SpawnPopup(GameObject popupPrefab)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("ShowPopup: popupPrefab�� �������");
            return null;
        }

        var popup = SpawnUI(popupPrefab, YSJ_UIType.Popup);

        // RectTransform ��ġ �ʱ�ȭ (Ǯ��ũ�� �߾�)
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

    // spawn for resource
    public static GameObject SpawnPopup(string resourcePath)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        return SpawnPopup(prefab);
    }
}
