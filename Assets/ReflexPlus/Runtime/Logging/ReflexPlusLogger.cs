using System;
using UnityEngine;

namespace ReflexPlus.Logging
{
    internal static class ReflexPlusLogger
    {
        private static LogLevel logLevel;

        static ReflexPlusLogger()
        {
            Log($"Reflex+ LogLevel set to {logLevel}", LogLevel.Info);
        }

        public static void UpdateLogLevel(LogLevel logLevel)
        {
            if (logLevel != ReflexPlusLogger.logLevel)
            {
                ReflexPlusLogger.logLevel = logLevel;
                Log($"Reflex+ LogLevel set to {ReflexPlusLogger.logLevel}", LogLevel.Info);
            }
        }

        public static void Log(object message, LogLevel logLevel, UnityEngine.Object context = null)
        {
            if (logLevel < ReflexPlusLogger.logLevel)
            {
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Development:
                case LogLevel.Info:
                    Debug.Log(message, context);
                    break;

                case LogLevel.Warning:
                    Debug.LogWarning(message, context);
                    break;

                case LogLevel.Error:
                    Debug.LogError(message, context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}