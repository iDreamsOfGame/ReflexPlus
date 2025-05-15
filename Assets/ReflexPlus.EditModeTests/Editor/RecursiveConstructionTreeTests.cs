using System;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Core;

// ReSharper disable NotAccessedField.Local

namespace ReflexPlus.EditModeTests
{
    public class RecursiveConstructionTreeTests
    {
        public class ServiceOne
        {
            private readonly float a;

            private readonly ServiceTwo b;

            public ServiceOne(float a, ServiceTwo b)
            {
                this.a = a;
                this.b = b;
            }
        }

        public class ServiceTwo
        {
            private readonly int a;

            private readonly int b;

            public ServiceTwo(int a, int b)
            {
                this.a = a;
                this.b = b;
            }
        }

        [Test]
        public void Resolve_RecursiveConstructionTree_ShouldNotThrow()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterValue(1.5f)
                .RegisterType(typeof(ServiceOne))
                .RegisterType(typeof(ServiceTwo))
                .Build();

            Action resolve = () => container.Resolve<ServiceOne>();
            resolve.Should().NotThrow();
        }
    }
}