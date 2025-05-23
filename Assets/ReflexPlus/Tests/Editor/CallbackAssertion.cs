using System;
using NUnit.Framework;

namespace ReflexPlusEditor.Tests
{
    internal class CallbackAssertion
    {
        private int calls;

        public void Invoke()
        {
            calls++;
        }

        public static implicit operator Action(CallbackAssertion callbackAssertion)
        {
            return callbackAssertion.Invoke;
        }

        public void ShouldHaveBeenCalledOnce()
        {
            Assert.That(calls, Is.EqualTo(1));
        }

        public void ShouldNotHaveBeenCalled()
        {
            Assert.That(calls, Is.EqualTo(0));
        }

        public void ShouldHaveBeenCalled(int times)
        {
            Assert.That(calls, Is.EqualTo(times));
        }
    }
}