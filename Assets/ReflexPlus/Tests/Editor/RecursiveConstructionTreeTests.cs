using NUnit.Framework;
using ReflexPlus.Core;

// ReSharper disable NotAccessedField.Local

namespace ReflexPlusEditor.Tests
{
    internal class RecursiveConstructionTreeTests
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
        public void Resolve_RecursiveConstructionTree_DoesNotThrowAnyException()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterValue(1.5f)
                .RegisterType(typeof(ServiceOne))
                .RegisterType(typeof(ServiceTwo))
                .Build();
            
            Assert.DoesNotThrow(() => container.Resolve<ServiceOne>());
        }
    }
}