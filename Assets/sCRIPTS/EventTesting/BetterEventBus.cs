using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        var type = typeof(T);
        if (!events.ContainsKey(type))
            events[type] = null;

        events[type] = (Action<T>)events[type] + listener;
    }

    public static void UnSubscribe<T>(Action<T> listener)
    {
        var type = typeof(T);
        if (events.ContainsKey(type))
            events[type] = (Action<T>)events[type] - listener;
    }

    public static void Invoke<T>(T evt)
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var del))
            ((Action<T>)del)?.Invoke(evt);
    }
}
