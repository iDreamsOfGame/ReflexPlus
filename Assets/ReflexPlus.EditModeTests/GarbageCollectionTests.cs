﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UnityEditor.Compilation;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    internal class GarbageCollectionTests
    {
        private class Service
        {
        }

        public static void ForceGarbageCollection()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            if (CompilationPipeline.codeOptimization == CodeOptimization.Debug)
            {
                Assert.Inconclusive("GC works differently in Debug mode. Please run tests in Release mode.");
            }
        }

        [Test, Retry(3)]
        public void Singleton_ShouldBeFinalized_WhenOwnerIsDisposed()
        {
            var references = new List<WeakReference>();

            void Act()
            {
                var container = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Lazy).Build();
                var service = container.Single<Service>();
                references.Add(new WeakReference(service));
                container.Dispose();
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test, Retry(3)]
        public void DisposedScopedContainer_ShouldHaveNoReferencesToItself_AndShouldBeCollectedAndFinalized()
        {
            var references = new List<WeakReference>();

            void Act()
            {
                var parent = new ContainerBuilder().Build();
                var scoped = parent.Scope();
                references.Add(new WeakReference(scoped));
                scoped.Dispose();
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test, Retry(3)]
        public void Construct_ContainerShouldNotControlConstructedObjectLifeCycle_ByNotKeepingReferenceToIt()
        {
            var references = new List<WeakReference>();
            var container = new ContainerBuilder().Build();

            void Act()
            {
                var service = container.Construct<Service>();
                references.Add(new WeakReference(service));
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }
    }
}