using System;
using System.Collections.Generic;

internal class DebugManager
{
    private static Boolean isDebugEnabled = true;
    private static Dictionary<Type, DebugLoggerLevel> levelByClass = new Dictionary<Type, DebugLoggerLevel>();

    static DebugManager()
    {
        //  levelByClass.Add(typeof(SwinBehaviour), DebugLoggerLevel.EXTREME);
        //    levelByClass.Add(typeof(SwimToBehaviour), DebugLoggerLevel.EXTREME);
    }

    // used for all classed that do not have a specific debug level set
    private static DebugLoggerLevel GLOBAL_LEVEL = DebugLoggerLevel.NORMAL;

    private static DebugManager INSTANCE;

    private static DebugManager getInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new DebugManager();
        }
        return INSTANCE;
    }

    public static DebugLogger getDebugLogger(Type type)
    {
        if (isDebugEnabled)
        {
            if (levelByClass.ContainsKey(type))
            {
                DebugLoggerLevel typeLevel;
                if (levelByClass.TryGetValue(type, out typeLevel))
                {
                    return new DebugLogger(typeLevel, type);
                }
            }
            return new DebugLogger(GLOBAL_LEVEL, type);
        }
        else
        {
            return new DebugLogger(DebugLoggerLevel.NONE, type);
        }
    }

    public static Boolean IsDebugEnabled()
    {
        return isDebugEnabled;
    }

}