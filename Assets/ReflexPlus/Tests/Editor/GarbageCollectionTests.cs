using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReflexPlus.Core;
using UnityEditor.Compilation;
using UnityEngine;

namespace ReflexPlusEditor.Tests
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
                Assert.Inconclusive("GC works differently in Debug mode. Please run tests in Release mode.");
        }

        [Test, Retry(3)]
        public void Singleton_ShouldBeFinalizedWhenOwnerIsDisposed_ReturnsFalse()
        {
            var references = new List<WeakReference>();

            Act();
            ForceGarbageCollection();
            Assert.That(references.Any(reference => reference.IsAlive), Is.False);
            return;

            void Act()
            {
                var container = new ContainerBuilder().RegisterType(typeof(Service)).Build();
                var service = container.Single<Service>();
                references.Add(new WeakReference(service));
                container.Dispose();
            }
        }

        [Test, Retry(3)]
        public void DisposedScopedContainer_ShouldHaveNoReferencesToItselfAndShouldBeCollectedAndFinalized_ReturnsFalse()
        {
            var references = new List<WeakReference>();

            Act();
            ForceGarbageCollection();
            Assert.That(references.Any(reference => reference.IsAlive), Is.False);
            return;

            void Act()
            {
                var parent = new ContainerBuilder().Build();
                var scoped = parent.Scope();
                references.Add(new WeakReference(scoped));
                scoped.Dispose();
            }
        }

        [Test, Retry(3)]
        public void Construct_ContainerShouldNotControlConstructedObjectLifeCycleByNotKeepingReferenceToIt_ReturnsFalse()
        {
            var references = new List<WeakReference>();
            var container = new ContainerBuilder().Build();

            Act();
            ForceGarbageCollection();
            Assert.That(references.Any(reference => reference.IsAlive), Is.False);
            return;

            void Act()
            {
                var service = container.Construct<Service>();
                references.Add(new WeakReference(service));
            }
        }
    }
}