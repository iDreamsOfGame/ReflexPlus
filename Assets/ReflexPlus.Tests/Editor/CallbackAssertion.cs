using System;
using FluentAssertions;

namespace ReflexPlus.EditModeTests
{
    public class CallbackAssertion
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
            calls.Should().Be(1);
        }

        public void ShouldNotHaveBeenCalled()
        {
            calls.Should().Be(0);
        }

        public void ShouldHaveBeenCalled(int times)
        {
            calls.Should().Be(times);
        }
    }
}