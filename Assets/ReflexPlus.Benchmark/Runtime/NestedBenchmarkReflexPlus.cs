using ReflexPlus.Benchmark.NestedModel;
using ReflexPlus.Benchmark.Utilities;
using ReflexPlus.Core;

namespace ReflexPlus.Benchmark
{
    internal class NestedBenchmarkReflexPlus : MonoProfiler
    {
        private Container container;

        private void Start()
        {
            container = new ContainerBuilder()
                .RegisterType(typeof(A), new[] { typeof(IA) }, Lifetime.Transient)
                .RegisterType(typeof(B), new[] { typeof(IB) }, Lifetime.Transient)
                .RegisterType(typeof(C), new[] { typeof(IC) }, Lifetime.Transient)
                .RegisterType(typeof(D), new[] { typeof(ID) }, Lifetime.Transient)
                .RegisterType(typeof(E), new[] { typeof(IE) }, Lifetime.Transient)
                .Build();
        }

        protected override void Sample()
        {
            container.Resolve<IA>();
        }
    }
}