using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class SingletonValueResolver : IResolver
    {
        private readonly DisposableCollection disposables = new();
        
        private readonly object value;

        public SingletonValueResolver(object value)
        {
            Diagnosis.RegisterCallSite(this);
            Diagnosis.RegisterInstance(this, value);
            this.value = value;
            disposables.TryAdd(value);
        }
        
        public Lifetime Lifetime => Lifetime.Singleton;

        public Resolution Resolution => Resolution.Lazy;

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);
            return value;
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}