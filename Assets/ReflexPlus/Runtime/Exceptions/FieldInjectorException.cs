using System;

namespace ReflexPlus.Exceptions
{
    internal sealed class FieldInjectorException : Exception
    {
        public FieldInjectorException(Exception e)
            : base(e.Message)
        {
        }
    }
}