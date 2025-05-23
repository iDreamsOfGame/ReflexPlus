using System;
using NUnit.Framework;
using ReflexPlus.Reflectors;

namespace ReflexPlusEditor.Tests
{
    internal class ActivatorFactoryTests
    {
        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ValueType_ReturnsCorrectValue(Type activatorFactoryType)
        {
            var activatorFactory = (IActivatorFactory)Activator.CreateInstance(activatorFactoryType);
            var activator = activatorFactory.GenerateDefaultActivator(typeof(int));
            var number = (int)activator.Invoke(null);
            Assert.That(number, Is.EqualTo(0));
        }

        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ReferenceType_ReturnsCorrectValue(Type activatorFactoryType)
        {
            var activatorFactory = (IActivatorFactory)Activator.CreateInstance(activatorFactoryType);
            var activator = activatorFactory.GenerateActivator(typeof(string), typeof(string).GetConstructor(new[] { typeof(char[]) }), new[] { typeof(char[]) });
            var complex = (string)activator.Invoke(Array.Empty<char>());
            Assert.That(complex, Is.Not.Null);
            Assert.That(complex, Is.EqualTo(string.Empty));
        }
    }
}