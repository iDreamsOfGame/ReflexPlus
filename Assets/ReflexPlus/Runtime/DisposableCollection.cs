using System;
using System.Collections.Generic;

namespace ReflexPlus
{
    internal sealed class DisposableCollection : IDisposable
    {
        private readonly Stack<IDisposable> stack = new();

        public void Add(IDisposable disposable)
        {
            stack.Push(disposable);
        }

        public void TryAdd(object obj)
        {
            if (obj is IDisposable disposable)
            {
                stack.Push(disposable);
            }
        }

        public void Dispose()
        {
            while (stack.TryPop(out var disposable))
            {
                disposable.Dispose();
            }
        }
    }
}