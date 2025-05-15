using NUnit.Framework;
using ReflexPlus.Tests;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ReflexPlus.EditorTests
{
    [SetUpFixture]
    public class PlayModeTestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(new TestResultReporter());
        }
    }
}