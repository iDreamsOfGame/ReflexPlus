using NUnit.Framework;
using ReflexPlus.Tests;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ReflexPlusEditor.Tests
{
    [SetUpFixture]
    internal class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(new TestResultReporter());
        }
    }
}