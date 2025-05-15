using NUnit.Framework;
using ReflexPlus.PlayModeTests;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ReflexPlus.EditModeTests
{
    [SetUpFixture]
    public class EditModeTestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(new TestResultReporter());
        }
    }
}