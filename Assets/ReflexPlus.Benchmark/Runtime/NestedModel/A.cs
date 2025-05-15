// ReSharper disable NotAccessedField.Local

namespace ReflexPlus.Benchmark.NestedModel
{
    internal class A : IA
    {
        private readonly IB b;

        public A(IB b)
        {
            this.b = b;
        }
    }
}