using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class JHT_BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> uiObjectDic;
    private Dictionary<string, Component> componentDic;

    private void Awake()
    {
        RectTransform[] recTrans = GetComponentsInChildren<RectTransform>(true);
        uiObjectDic = new Dictionary<string, GameObject>(recTrans.Length * 4);

        foreach (RectTransform child in recTrans)
        {
            uiObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<string,Component>(components.Length * 4);
    
        foreach (Component com in components)
        {
            componentDic.TryAdd($"{com.gameObject}_{com.GetType().Name}", com);
        }
    }

    public GameObject GetUI(in string objName)
    {
        uiObjectDic.TryGetValue(objName, out GameObject value);
        return value;
    }

    public T GetUI<T>(in string objName) where T : Component
    {
        componentDic.TryGetValue($"{objName}_{typeof(T).Name}", out Component com);

        if (com != null) 
            return com as T;

        GameObject obj = GetUI(objName);

        if (obj == null)
            return null;

        com = obj.GetComponent<T>();

        if (com == null)
            return null;

        componentDic.TryAdd($"{objName}_{typeof(T).Name}", com);
        return com as T;

    }

    public JHT_PointerHandler GetEvent(in string objName)
    {
        JHT_PointerHandler obj = GetUI(objName).GetOrAddComponent<JHT_PointerHandler>();
        return obj;
    }
}
