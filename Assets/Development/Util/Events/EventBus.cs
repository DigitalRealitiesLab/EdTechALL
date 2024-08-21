using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<string, List<EventCallback>> callbacksByEventId = new Dictionary<string, List<EventCallback>>();
    private static List<EventCallback> subscriberCallbacks = new List<EventCallback>();

    private static bool HasCallback(object subscriber, string eventId, string callbackMethodName)
    {
        return GetCallback(subscriber, eventId, callbackMethodName) != null;
    }

    private static EventCallback GetCallback(object subscriber, string eventId, string callbackMethodName)
    {
        bool finished = false;
        while (!finished)
        {
            if (callbacksByEventId.ContainsKey(eventId))
            {
                try
                {
                    foreach (var callback in callbacksByEventId[eventId])
                    {
                        if (callback.subscriber == subscriber && callback.callbackMethodName == callbackMethodName)
                        {
                            return callback;
                        }
                    }
                    finished = true;
                }
                catch (InvalidOperationException)
                {
                    finished = false;
                }
            }
            else
            {
                finished = true;
            }
        }
        return null;
    }

    private static void RegisterCallback(EventCallback callback)
    {
        if (!callbacksByEventId.ContainsKey(callback.eventId))
        {
            callbacksByEventId.Add(callback.eventId, new List<EventCallback>());
        }
        callbacksByEventId[callback.eventId].Add(callback);
    }

    private static void RegisterCallback(object listener, string eventId, string callbackMethodName)
    {
        RegisterCallback(new EventCallback(listener, eventId, callbackMethodName));
    }

    public static void SaveRegisterCallback(object listener, string eventId, string callbackMethodName)
    {
        if (!HasCallback(listener, eventId, callbackMethodName))
        {
            RegisterCallback(listener, eventId, callbackMethodName);
        }
    }

    private static void DeregisterCallback(EventCallback callback)
    {
        callbacksByEventId[callback.eventId].Remove(callback);
    }

    private static void DeregisterCallback(object subscriber, string eventId, string callbackMethodName)
    {
        EventCallback eventCallback = GetCallback(subscriber, eventId, callbackMethodName);
        if (eventCallback != null)
        {
            DeregisterCallback(eventCallback);
        }
        else
        {
            Debug.LogWarning("Failed to deregister event callback to " + eventId + " to subscriber " + subscriber.ToString() + " and callback method " + callbackMethodName);
        }
    }

    public static void SaveDeregisterCallback(object listener, string eventId, string callbackMethodName)
    {
        if (HasCallback(listener, eventId, callbackMethodName))
        {
            DeregisterCallback(listener, eventId, callbackMethodName);
        }
    }

    public static void DeregisterAllCallbacks(object subscriber)
    {
        GetCallbacksOfSubsciber(subscriber);
        bool finished = false;
        while (!finished)
        {
            try
            {
                foreach (var callbackBySub in subscriberCallbacks)
                {
                    foreach (var callbacksById in callbacksByEventId)
                    {
                        if (callbacksById.Value.Contains(callbackBySub))
                        {
                            callbacksById.Value.Remove(callbackBySub);
                        }
                    }
                }
                finished = true;
            }
            catch (InvalidOperationException)
            {
                finished = false;
            }
        }
    }

    public static void Publish(string eventId, params object[] list)
    {
        EventCallback lastCallback = null;
        bool reachedLastCallback = true;
        bool finished = false;
        while (!finished)
        {
            if (callbacksByEventId.ContainsKey(eventId))
            {
                try
                {
                    foreach (var callback in callbacksByEventId[eventId])
                    {
                        if (reachedLastCallback || lastCallback == null)
                        {
                            try
                            {
                                callback.method.Invoke(callback.subscriber, list);
                                lastCallback = callback;
                            }
                            catch (TargetParameterCountException)
                            {
                                Debug.LogError("Parameter Count Mismatch: Could not invoke method " + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString());
                            }
                            catch (ArgumentException)
                            {
                                Debug.LogError("Argument Type Mismatch: Could not invoke method " + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString());
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e.Message + "(" + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString() + ")");
                            }
                        }
                        else if (lastCallback == callback)
                        {
                            reachedLastCallback = true;
                        }
                    }
                    finished = true;
                }
                catch (InvalidOperationException)
                {
                    finished = false;
                    reachedLastCallback = false;
                }
            }
            else
            {
                finished = true;
            }
        }
    }

    public static IEnumerator PublishCoroutine(string eventId, params object[] list)
    {
        yield return new WaitUntil(() => CheckCallbacks(eventId, list));
    }

    public static bool CheckCallbacks(string eventId, params object[] list)
    {
        EventCallback lastCallback = null;
        bool reachedLastCallback = true;
        bool finished = false;
        while (!finished)
        {
            try
            {
                foreach (var callback in callbacksByEventId[eventId])
                {
                    if (reachedLastCallback || lastCallback == null)
                    {
                        try
                        {
                            callback.method.Invoke(callback.subscriber, list);
                            lastCallback = callback;
                        }
                        catch (TargetParameterCountException)
                        {
                            Debug.LogError("Parameter Count Mismatch: Could not invoke method " + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString());
                        }
                        catch (ArgumentException)
                        {
                            Debug.LogError("Argument Type Mismatch: Could not invoke method " + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message + "(" + callback.callbackMethodName + " on subscriber " + callback.subscriber.ToString() + ")");
                        }
                    }
                    else if (lastCallback == callback)
                    {
                        reachedLastCallback = true;
                    }
                }
                finished = true;
            }
            catch (InvalidOperationException)
            {
                finished = false;
                reachedLastCallback = false;
            }
        }

        return true;
    }

    private static void GetCallbacksOfSubsciber(object subscriber)
    {
        bool finished = false;
        while (!finished)
        {
            subscriberCallbacks.Clear();
            try
            {
                foreach (var callbackList in callbacksByEventId)
                {
                    foreach (var callback in callbackList.Value)
                    {
                        if (callback.subscriber == subscriber)
                        {
                            subscriberCallbacks.Add(callback);
                        }
                    }
                }
                finished = true;
            }
            catch (InvalidOperationException)
            {
                finished = false;
            }
        }
    }
}

public class EventCallback
{
    public object subscriber { private set; get; }
    public string eventId { private set; get; }
    public string callbackMethodName { private set; get; }
    public MethodBase method { private set; get; }

    public EventCallback(object subscriber, string eventId, string callbackMethodName)
    {
        this.subscriber = subscriber;
        this.eventId = eventId;
        this.callbackMethodName = callbackMethodName;

        Type type = subscriber.GetType();
        method = type.GetMethod(callbackMethodName);
        Debug.Assert(method != null, "Object " + subscriber.ToString() + " tried to reigster to event " + eventId + " but does not specify a public method with the name " + callbackMethodName);
    }
}
