// ReSharper disable NotAccessedField.Local

namespace ReflexPlus.Benchmark.NestedModel
{
    internal class B : IB
    {
        private readonly IC c;

        public B(IC c)
        {
            this.c = c;
        }
    }
}