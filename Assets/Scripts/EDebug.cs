using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class help create debug message that only appear in the editor 
 */
public static class EDebug
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object msg, Object context)
    {
        Debug.Log(msg, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object msg)
    {
        Debug.LogError(msg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object msg, Object context)
    {
        Debug.LogError(msg, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogAssertion(object msg, Object context)
    {
        Debug.LogAssertion(msg, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogAssertion(object msg)
    {
        Debug.LogAssertion(msg);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition)
    {
        Debug.Assert(condition);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, Object context)
    {
        Debug.Assert(condition, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, object message)
    {
        Debug.Assert(condition, message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, object message, Object context)
    {
        Debug.Assert(condition, message, context);
    }

}