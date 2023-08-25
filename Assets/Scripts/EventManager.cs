using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    private static Dictionary<string, Action<object>> _gameEventDict = new Dictionary<string, Action<object>>();

    // Create an event and add to the dictionary
    public static void EventInitialise(string gameEventName)
    {
        Action<object> newGameEvent = null;
        _gameEventDict.Add(gameEventName, newGameEvent);
    }

    // Subscribe function handler to event
    public static void EventSubscribe(string gameEventName, Action<object> funcToSub)
    {
        // Check if event exists, then sub handler function to it
        if (_gameEventDict.ContainsKey(gameEventName))
        {
            _gameEventDict[gameEventName] += funcToSub;
        }
    }

    // Unsubscribe function handler from event
    public static void EventUnsubscribe(string gameEventName, Action<object> funcToUnsub)
    {
        // Check if event exists, then unsub handler function from it
        if (_gameEventDict.ContainsKey(gameEventName))
        {
            _gameEventDict[gameEventName] -= funcToUnsub;
        }
    }

    // Trigger event
    public static void EventTrigger(string gameEventName, object data)
    {
        // Check if event exists, then invoke and execute all handlers subscribed to it
        if (_gameEventDict.ContainsKey(gameEventName))
        {
            _gameEventDict[gameEventName]?.Invoke(data);
        }
    }
}
