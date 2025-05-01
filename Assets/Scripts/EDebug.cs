using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class help create debug message that only appear in the editor and are removed in the release build 
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
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(message, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        Debug.LogWarningFormat(format, args);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarningFormat(Object context, string format, params object[] args)
    {
        Debug.LogWarningFormat(context, format, args);
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
    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogException(System.Exception exception, Object context)
    {
        Debug.LogException(exception, context);
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

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void AssertFormat(bool condition, string format, params object[] args)
    {
        Debug.AssertFormat(condition, format, args);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void AssertFormat(bool condition, Object context, string format, params object[] args)
    {
        Debug.AssertFormat(condition, context, format, args);
    }

}
