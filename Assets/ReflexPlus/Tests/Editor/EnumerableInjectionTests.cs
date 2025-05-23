using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReflexPlus.Core;

// ReSharper disable ClassNeverInstantiated.Local

namespace ReflexPlusEditor.Tests
{
    internal class EnumerableInjectionTests
    {
        private class NumberManager
        {
            public IEnumerable<int> Numbers { get; }

            public NumberManager(IEnumerable<int> numbers)
            {
                Numbers = numbers;
            }
        }

        [Test]
        public void Container_ShouldBeAbleToConstructObjectWithIEnumerableDependency_DoesNotThrowAnyException()
        {
            var container = new ContainerBuilder()
                .RegisterValue(1)
                .RegisterValue(2)
                .RegisterValue(3)
                .Build();
            
            Assert.DoesNotThrow(() => container.Construct<NumberManager>());
        }

        [Test]
        public void NestedEnumerableShouldBeSupported_IsEquivalentToExpectedCollection()
        {
            var container = new ContainerBuilder()
                .RegisterValue(new List<int> { 1, 2, 3 })
                .RegisterValue(new List<int> { 4, 5, 6 })
                .Build();

            Assert.That(container.All<List<int>>().SelectMany(x => x), Is.EquivalentTo(new[] { 1, 2, 3, 4, 5, 6 }));
        }
    }
}