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
                .RegisterType(typeof(A), typeof(IA), Lifetime.Transient)
                .RegisterType(typeof(B), typeof(IB), Lifetime.Transient)
                .RegisterType(typeof(C), typeof(IC), Lifetime.Transient)
                .RegisterType(typeof(D), typeof(ID), Lifetime.Transient)
                .RegisterType(typeof(E), typeof(IE), Lifetime.Transient)
                .Build();
        }

        protected override void Sample()
        {
            container.Resolve<IA>();
        }
    }
}