namespace ReflexPlus.Il2cppTests.Models
{
    public interface ITestGenericStructure<T> where T : struct
    {
        T Value { get; set; }
    }
}