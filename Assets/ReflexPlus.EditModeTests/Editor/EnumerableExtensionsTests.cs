using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Extensions;

namespace ReflexPlus.EditModeTests
{
    public class EnumerableExtensionsTests
    {
        private static readonly IEnumerable<object> Numbers = new List<object> { 1, 2, 3, 42 };

        [Test]
        public void AfterDynamicallyCasted_ShouldBeAssignable()
        {
            Numbers.CastDynamic(typeof(int)).GetType().Should().BeAssignableTo<IEnumerable<int>>();
        }

        [Test]
        public void AfterDynamicallyCasted_ShouldBeConvertible()
        {
            string.Join(",", (IEnumerable<int>)Numbers.CastDynamic(typeof(int))).Should().Be("1,2,3,42");
        }
    }
}