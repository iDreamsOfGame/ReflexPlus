// ReSharper disable NotAccessedField.Local

namespace ReflexPlus.Benchmark.NestedModel
{
    internal class C : IC
    {
        private readonly ID d;

        public C(ID d)
        {
            this.d = d;
        }
    }
}