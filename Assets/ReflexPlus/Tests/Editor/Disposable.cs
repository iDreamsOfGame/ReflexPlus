using System;

namespace ReflexPlusEditor.Tests
{
    internal class Disposable : IDisposable
    {
        private readonly Action onDispose;

        private Disposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            onDispose?.Invoke();
        }

        public static IDisposable Create(Action onDispose)
        {
            return new Disposable(onDispose);
        }
    }
}