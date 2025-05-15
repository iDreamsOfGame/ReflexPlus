using ReflexPlus.Benchmark.NestedModel;
using ReflexPlus.Benchmark.Utilities;
using VContainer;

namespace ReflexPlus.Benchmark
{
    internal class NestedBenchmarkVContainer : MonoProfiler
    {
        private readonly ContainerBuilder containerBuilder = new();

        private IObjectResolver objectResolver;

        private void Start()
        {
            containerBuilder.Register<IA, A>(VContainer.Lifetime.Transient);
            containerBuilder.Register<IB, B>(VContainer.Lifetime.Transient);
            containerBuilder.Register<IC, C>(VContainer.Lifetime.Transient);
            containerBuilder.Register<ID, D>(VContainer.Lifetime.Transient);
            containerBuilder.Register<IE, E>(VContainer.Lifetime.Transient);
            objectResolver = containerBuilder.Build();
        }

        protected override void Sample()
        {
            objectResolver.Resolve<IA>();
        }
    }
}