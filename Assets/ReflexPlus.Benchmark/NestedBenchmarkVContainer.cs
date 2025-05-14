using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;
using VContainer;

namespace Reflex.Benchmark
{
    internal class NestedBenchmarkVContainer : MonoProfiler
    {
        private readonly ContainerBuilder _containerBuilder = new();
        private IObjectResolver _objectResolver;

        private void Start()
        {
            _containerBuilder.Register<IA, A>(Lifetime.Transient);
            _containerBuilder.Register<IB, B>(Lifetime.Transient);
            _containerBuilder.Register<IC, C>(Lifetime.Transient);
            _containerBuilder.Register<ID, D>(Lifetime.Transient);
            _containerBuilder.Register<IE, E>(Lifetime.Transient);
            _objectResolver = _containerBuilder.Build();
        }

        protected override void Sample()
        {
            _objectResolver.Resolve<IA>();
        }
    }
}