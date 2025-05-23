using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlus.Extensions;

namespace ReflexPlusEditor.Tests
{
    internal class TypeExtensionsTests
    {
        [Test]
        public void IsEnumerable_ReturnsTrue_ForGenericIEnumerableDefinition()
        {
            Assert.That(typeof(IEnumerable<int>).IsEnumerable(out _), Is.True);
        }

        [Test]
        public void IsEnumerable_ShouldReturnFalse_ForAnythingElse()
        {
            Assert.That(typeof(Dictionary<int, string>).IsEnumerable(out _), Is.False);
        }
    }
}