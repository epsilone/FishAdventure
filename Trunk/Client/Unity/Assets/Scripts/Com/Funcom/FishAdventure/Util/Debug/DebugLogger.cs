using System;
using UnityEngine;

internal class DebugLogger : IDebugLogger
{
    private DebugLoggerLevel level;
    private Type loggedType;

    public DebugLogger(DebugLoggerLevel level, Type type)
    {
        this.level = level;
        this.loggedType = type;
    }

    public void Log(string text)
    {
        Log(text, Severity.INFO, null);
    }

    public void Log(string text, Severity severity, Exception e)
    {
        if (isEnabled())
        {
            String message = loggedType.Name + " :: " + text;

            if (e != null)
            {
                message += "  \n  " + StackTraceUtility.ExtractStringFromException(e);
            }

            switch (severity)
            {
                case Severity.ERROR:
                    Debug.LogError(text);
                    DebugConsole.LogError(text);
                    break;

                case Severity.WARN:
                    Debug.LogWarning(text);
                    DebugConsole.LogWarning(text);
                    break;

                default:
                    Debug.Log(text);
                    DebugConsole.Log(text);
                    break;
            }
        }
    }

    public void Log(string text, Severity severity)
    {
        Log(text, severity, null);
    }

    public void Log(string text, Exception e)
    {
        Log(text, Severity.INFO, e);
    }

    public void LogWarning(string text)
    {
        Log(text, Severity.WARN, null);
    }

    public void LogWarning(string text, Exception e)
    {
        Log(text, Severity.WARN, e);
    }

    public void LogError(string text)
    {
        Log(text, Severity.ERROR, null);
    }

    public void LogError(string text, Exception e)
    {
        Log(text, Severity.ERROR, e);
    }

    private Boolean isEnabled()
    {
        return !DebugLoggerLevel.NONE.Equals(level);
    }

    public bool IsExtreme()
    {
        return DebugLoggerLevel.EXTREME.Equals(level);
    }
}