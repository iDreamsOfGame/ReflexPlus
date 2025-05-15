using ReflexPlus.Benchmark.NestedModel;
using ReflexPlus.Benchmark.Utilities;
using Zenject;

namespace ReflexPlus.Benchmark
{
    internal class NestedBenchmarkZenject : MonoProfiler
    {
        private readonly DiContainer container = new();

        private void Start()
        {
            container.Bind<IA>().To<A>().AsTransient();
            container.Bind<IB>().To<B>().AsTransient();
            container.Bind<IC>().To<C>().AsTransient();
            container.Bind<ID>().To<D>().AsTransient();
            container.Bind<IE>().To<E>().AsTransient();
        }

        protected override void Sample()
        {
            container.Resolve<IA>();
        }
    }
}