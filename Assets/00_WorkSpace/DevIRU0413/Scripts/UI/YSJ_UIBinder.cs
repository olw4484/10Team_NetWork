using System;
using System.Collections.Generic;
using UnityEngine;

public class YSJ_UIBinder<TEnum> where TEnum : Enum
{
    private readonly Dictionary<TEnum, string> _pathMap = new();
    private readonly JHT_BaseUI _uiContext;

    public YSJ_UIBinder(JHT_BaseUI uiContext)
    {
        _uiContext = uiContext;
        CachePaths();
    }

    private void CachePaths()
    {
        foreach (TEnum key in Enum.GetValues(typeof(TEnum)))
        {
            _pathMap[key] = key.ToString();
        }
    }
    
    public GameObject Get(TEnum key)
    {
        return _uiContext.GetUI(_pathMap[key]);
    }

    public T Get<T>(TEnum key) where T : Component
    {
        return _uiContext.GetUI<T>(_pathMap[key]);
    }

    public JHT_PointerHandler GetEvent(TEnum key)
    {
        return _uiContext.GetEvent(_pathMap[key]);
    }
}
