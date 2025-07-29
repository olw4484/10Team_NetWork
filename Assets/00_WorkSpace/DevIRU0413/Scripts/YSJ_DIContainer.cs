using System;
using System.Collections.Generic;
using Scripts.Util;

public class YSJ_DIContainer : YSJ_SimpleSingleton<YSJ_DIContainer>
{
    private Dictionary<Type, object> serviceMap = new();

    public void Register<T>(T instance)
    {
        serviceMap[typeof(T)] = instance;
    }

    public T Resolve<T>()
    {
        if (serviceMap.TryGetValue(typeof(T), out var service))
            return (T)service;
        else
            throw new Exception($"DIContainer에 {typeof(T)} 가 등록되지 않았습니다.");
    }

    public void Clear()
    {
        serviceMap.Clear();
    }
}
