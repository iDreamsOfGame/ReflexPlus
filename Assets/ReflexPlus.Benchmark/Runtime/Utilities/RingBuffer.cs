using System;

namespace ReflexPlus.Benchmark.Utilities
{
    internal class RingBuffer<T>
    {
        private int offset;

        private readonly T[] array;

        public int Capacity => array.Length;

        public int Length { get; private set; }

        public T this[int i] => array[Circle(offset - i, Length)];

        public RingBuffer(int capacity)
        {
            ValidateCapacity(capacity);
            array = new T[capacity];
            offset = -1;
        }

        public void Push(T element)
        {
            offset = (offset + 1) % Capacity;
            array[offset] = element;
            Length = Math.Min(Length + 1, Capacity);
        }

        private static int Circle(int number, int rangeExclusive)
        {
            var result = number % rangeExclusive;

            if ((result < 0 && rangeExclusive > 0) || (result > 0 && rangeExclusive < 0))
            {
                result += rangeExclusive;
            }

            return result;
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    capacity,
                    "Capacity should not be less or equal to zero.");
            }
        }
    }
}