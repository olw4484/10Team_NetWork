using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBinder<TEnum> where TEnum : Enum
{
    private readonly Dictionary<TEnum, string> _pathMap = new();
    private readonly JHT_BaseUI _uiContext;

    public UIBinder(JHT_BaseUI uiContext)
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

    public T Get<T>(TEnum key) where T : Component
    {
        return _uiContext.GetUI<T>(_pathMap[key]);
    }

    public GameObject Get(TEnum key)
    {
        return _uiContext.GetUI(_pathMap[key]);
    }
}
