using System;
using UnityEngine;

namespace ReflexPlus.PlayModeTests
{
    public class ApplicationStackTraceLogTypeScope : IDisposable
    {
        private readonly LogType logType;

        private readonly StackTraceLogType previousStackTraceLogType;

        public ApplicationStackTraceLogTypeScope(LogType logType, StackTraceLogType stackTraceLogType)
        {
            this.logType = logType;
            previousStackTraceLogType = Application.GetStackTraceLogType(logType);
            Application.SetStackTraceLogType(logType, stackTraceLogType);
        }

        public void Dispose()
        {
            Application.SetStackTraceLogType(logType, previousStackTraceLogType);
        }
    }
}