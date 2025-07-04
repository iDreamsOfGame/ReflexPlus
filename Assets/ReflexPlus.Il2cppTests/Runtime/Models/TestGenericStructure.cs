﻿namespace ReflexPlus.Il2cppTests.Models
{
    public struct TestGenericStructure<T> : ITestGenericStructure<T> where T : struct
    {
        public T Value { get; set; }

        public TestGenericStructure(T internalValue)
        {
            Value = internalValue;
        }
    }
}