using UnityEngine;

using System;
using System.Collections.Generic;

public static class EventBus
{
    // Dictionary: EventType → Delegate
    private static readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

    // Subscribe to an event
    public static void Subscribe<T>(Action<T> listener)
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            eventTable[typeof(T)] = (Action<T>)existingDelegate + listener;
        }
        else
        {
            eventTable[typeof(T)] = listener;
        }
    }

    // Unsubscribe from an event
    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            var newDelegate = (Action<T>)existingDelegate - listener;

            if (newDelegate == null)
                eventTable.Remove(typeof(T));
            else
                eventTable[typeof(T)] = newDelegate;
        }
    }

    // Publish an event
    public static void Publish<T>(T eventData)
    {
        if (eventTable.TryGetValue(typeof(T), out var existingDelegate))
        {
            (existingDelegate as Action<T>)?.Invoke(eventData);
        }
    }
}
