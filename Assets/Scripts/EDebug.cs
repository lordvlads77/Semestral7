using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class help create debug message that only appear in the editor 
 */
public static class EDebug
{
    public static void Log(object msg)
    {
        #if UNITY_EDITOR
        Debug.Log(msg);
        #endif // UNITY_EDITOR
    }

    public static void Log(object msg, Object context)
    {
        #if UNITY_EDITOR
        Debug.Log(msg, context);
        #endif // UNITY_EDITOR
    }

    public static void LogError(object msg)
    {
        #if UNITY_EDITOR
        Debug.LogError(msg);
        #endif // UNITY_EDITOR
    }

    public static void LogError(object msg, Object context)
    {
        #if UNITY_EDITOR
        Debug.LogError(msg, context);
        #endif // UNITY_EDITOR
    }

    public static void LogAssertion(object msg, Object context)
    {
        #if UNITY_EDITOR
        Debug.LogAssertion(msg, context);
        #endif // UNITY_EDITOR
    }

    public static void LogAssertion(object msg)
    {
        #if UNITY_EDITOR
        Debug.LogAssertion(msg);
        #endif // UNITY_EDITOR
    }

    public static void Assert(bool condition)
    {
        #if UNITY_EDITOR
        Debug.Assert(condition);
        #endif // UNITY_EDITOR
    }


    public static void Assert(bool condition, Object context)
    {
        #if UNITY_EDITOR
        Debug.Assert(condition, context);
        #endif // UNITY_EDITOR
    }

    public static void Assert(bool condition, object message)
    {
        #if UNITY_EDITOR
        Debug.Assert(condition, message);
        #endif // UNITY_EDITOR
    }

    public static void Assert(bool condition, object message, Object context)
    {
        #if UNITY_EDITOR
        Debug.Assert(condition, message, context);
        #endif // UNITY_EDITOR
    }

}
