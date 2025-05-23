using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlus.Extensions;

namespace ReflexPlusEditor.Tests
{
    internal class EnumerableExtensionsTests
    {
        private static readonly IEnumerable<object> Numbers = new List<object> { 1, 2, 3, 42 };

        [Test]
        public void AfterDynamicallyCasted_ShouldBeAssignable_IsAssignableToCorrectType()
        {
            Assert.That(Numbers.CastDynamic(typeof(int)), Is.AssignableTo<IEnumerable<int>>());
        }

        [Test]
        public void AfterDynamicallyCasted_ShouldBeConvertible_ReturnsCorrectValue()
        {
            Assert.That( string.Join(",", (IEnumerable<int>)Numbers.CastDynamic(typeof(int))), Is.EqualTo("1,2,3,42"));
        }
    }
}