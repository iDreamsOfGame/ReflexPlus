using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UnusedVariable

namespace ReflexPlusEditor.Tests
{
    internal class ScopedContainerTests
    {
        private class Disposable : IDisposable
        {
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        private class DisposeHook : IDisposable
        {
            private event Action OnDispose;

            public DisposeHook(Action onDispose)
            {
                OnDispose += onDispose;
            }

            public void Dispose()
            {
                OnDispose?.Invoke();
            }
        }

        [Test]
        public void ScopedContainer_CanResolveDependencyFromParentContainer_ReturnsExpectedValue()
        {
            using (var outer = new ContainerBuilder().RegisterValue(42).Build())
            {
                using (var inner = outer.Scope())
                {
                    Assert.That(inner.Single<int>(), Is.EqualTo(42));
                }
            }
        }

        [Test]
        public void ParentWithScopedContainer_ParentShouldNotBeAbleToResolveDependencyFromChild_ThrowsUnknownContractException()
        {
            var outer = new ContainerBuilder().Build();
            outer.Scope(builder => builder.RegisterValue(42));
            Assert.Throws<UnknownContractException>(() => outer.Single<int>());
        }

        [Test]
        public void ScopedContainer_WhenDisposedShouldNotDisposeDependencyFromParentContainer_ReturnsExpectedValue()
        {
            using (var outer = new ContainerBuilder().RegisterType(typeof(Disposable)).Build())
            {
                using (var inner = outer.Scope())
                {
                    var disposable = inner.Single<Disposable>();
                    Assert.That(disposable, Is.Not.Null);
                }

                Assert.That(outer.Single<Disposable>().Disposed, Is.EqualTo(0));
            }
        }

        [Test]
        public void ScopedContainer_ScopedShouldParentAsParent_ReturnsExpectedInstance()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    Assert.That(inner.Parent, Is.SameAs(outer));
                }
            }
        }

        [Test]
        public void ScopedContainer_ParentShouldHaveScopedAsChild_ReturnsTrue()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    Assert.That(outer.Children.Contains(inner), Is.True);
                }
            }
        }

        [Test]
        public void ScopedContainer_AfterDisposalShouldBeRemoveAsParentChild_ReturnsExpectedInstance()
        {
            var parent = new ContainerBuilder().Build();
            var child = parent.Scope();

            Assert.That(child.Parent, Is.SameAs(parent));
            Assert.That(parent.Children.Contains(child), Is.True);
            child.Dispose();
            Assert.That(parent.Children, Is.Empty);
        }

        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder1_ReturnsExpectedValue()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .RegisterValue(new DisposeHook(() => disposalOrder.Add("a")))
                .Build();

            var b = a.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("b"))));
            var c = b.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("c"))));
            var d = c.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("d"))));
            var e = d.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("e"))));

            a.Dispose();

            Assert.That(string.Join(",", disposalOrder), Is.EquivalentTo("e,d,c,b,a"));
        }

        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder2_ReturnsExpectedValue()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .RegisterValue(new DisposeHook(() => disposalOrder.Add("a")))
                .Build();

            var b = a.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("b"))));

            a.Dispose();

            Assert.That(string.Join(",", disposalOrder), Is.EquivalentTo("b,a"));
        }

        [Test]
        public void ScopedContainer_ResolvingContainerFromInnerScopeShouldResolveInner_ReturnsExpectedInstance()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    Assert.That(inner.Single<Container>(), Is.SameAs(inner));
                }
            }
        }

        [Test]
        public void ScopedContainer_ResolvingContainerFromOuterScope_ShouldResolveOuter_ReturnsExpectedInstance()
        {
            using var outer = new ContainerBuilder().Build();
            using var inner = outer.Scope();
            inner.Single<Container>();
            Assert.That(outer.Single<Container>(), Is.SameAs(outer));
        }
    }
}