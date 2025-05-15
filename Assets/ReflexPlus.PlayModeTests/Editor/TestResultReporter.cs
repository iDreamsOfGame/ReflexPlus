using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ReflexPlus.PlayModeTests
{
    public class TestResultReporter : ICallbacks
    {
        private readonly List<ITestResultAdaptor> results = new();

        void ICallbacks.RunStarted(ITestAdaptor testsToRun)
        {
            results.Clear();
        }

        void ICallbacks.RunFinished(ITestResultAdaptor result)
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.UnregisterCallbacks(this);

            using (new ApplicationStackTraceLogTypeScope(LogType.Log, StackTraceLogType.None))
            {
                ReportStatus(TestStatus.Passed);
                ReportStatus(TestStatus.Failed);
                ReportStatus(TestStatus.Skipped);
                ReportStatus(TestStatus.Inconclusive);
            }
        }

        void ICallbacks.TestStarted(ITestAdaptor test)
        {
        }

        void ICallbacks.TestFinished(ITestResultAdaptor result)
        {
            if (!result.Test.IsSuite && !result.Test.IsTestAssembly)
            {
                results.Add(result);
            }
        }

        private void ReportStatus(TestStatus status)
        {
            var dict = new Dictionary<TestStatus, string>
            {
                { TestStatus.Passed, "✅" },
                { TestStatus.Failed, "❌" },
                { TestStatus.Skipped, "⏭️" },
                { TestStatus.Inconclusive, "⭕" },
            };

            var report = results
                .Where(r => r.TestStatus == status)
                .Select(r => r.Name)
                .OrderBy(r => r.Length)
                .ToList();

            report.Insert(0, $"{dict[status]} [{status.ToString().ToUpper()} {report.Count}/{results.Count}]");

            Debug.Log(string.Join(Environment.NewLine, report));
        }
    }
}