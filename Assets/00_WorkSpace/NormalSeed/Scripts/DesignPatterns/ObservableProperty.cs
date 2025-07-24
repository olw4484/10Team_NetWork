using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObservableProperty<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;
            _value = value;
            Notify();
        }
    }

    private UnityEvent<T> onValueChanged = new();
    public ObservableProperty(T value = default)
    {
        _value = value;
    }

    public void Subscribe(UnityAction<T> action)
    {
        onValueChanged.AddListener(action);
    }

    public void Unsubscribe(UnityAction<T> action)
    {
        onValueChanged.RemoveListener(action);
    }

    public void UnsubscribeAll()
    {
        onValueChanged.RemoveAllListeners();
    }

    private void Notify()
    {
        onValueChanged?.Invoke(Value);
    }
}
