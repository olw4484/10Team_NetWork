using Runtime.UI;
using Unity.VisualScripting;
using UnityEngine;

public class YSJ_UISpawnFactory
{
    // spawn for prefab
    public static GameObject SpawnUI(GameObject prefab)
    {
        GameObject uiObject = GameObject.Instantiate(prefab);
        JHT_BaseUI uiComp = uiObject.GetOrAddComponent<JHT_BaseUI>();
        if (uiComp == null)
        {
            Debug.LogError($"[UISpawnFactory] UI component not found on prefab: {prefab.name}");
            return null;
        }
        return uiObject;
    }

    // spawn for resource
    public static GameObject SpawnUI(string resourcePath)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        if (prefab == null)
        {
            Debug.LogError($"[UISpawnFactory] Prefab '{resourcePath}' not found!");
            return null;
        }

        return SpawnUI(prefab);
    }

    // spawn for prefab
    public static GameObject SpawnPopup(GameObject popupPrefab)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("ShowPopup: popupPrefab이 비어있음");
            return null;
        }

        var popup = SpawnUI(popupPrefab);

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

    // spawn for resource
    public static GameObject SpawnPopup(string resourcePath)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        return SpawnPopup(prefab);
    }
}
