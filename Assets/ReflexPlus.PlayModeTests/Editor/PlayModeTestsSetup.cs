﻿using NUnit.Framework;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ReflexPlus.PlayModeTests
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