using System;

namespace ReflexPlus.Exceptions
{
    internal sealed class PropertyInjectorException : Exception
    {
        public PropertyInjectorException(Exception e)
            : base(e.Message)
        {
        }
    }
}