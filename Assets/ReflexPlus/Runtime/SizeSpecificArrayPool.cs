using System;

namespace ReflexPlus
{
    internal sealed class SizeSpecificArrayPool<T> where T : new()
    {
        public const int InitialBucketSize = 4;

        private readonly T[][][] buckets; // Similar to a Dictionary<int, Stack<T[]>> but all inlined for performance

        private readonly int[] rentals;

        public SizeSpecificArrayPool(int maxLength)
        {
            maxLength += 1;

            buckets = new T[maxLength][][];
            rentals = new int[maxLength];

            for (var length = 0; length < maxLength; length++)
            {
                var bucket = new T[InitialBucketSize][];

                for (var j = 0; j < bucket.Length; j++)
                {
                    bucket[j] = new T[length];
                }

                buckets[length] = bucket;
            }
        }

        public T[] Rent(int length)
        {
            var maxLengthSupported = buckets.Length - 1;
            if (length > maxLengthSupported)
            {
                return new T[length];
            }

            var bucket = buckets[length];
            var rentalIndex = rentals[length];

            if (rentalIndex >= bucket.Length)
            {
                var newSize = bucket.Length * 2;
                Array.Resize(ref bucket, newSize);

                for (var i = rentalIndex; i < newSize; i++)
                {
                    bucket[i] = new T[length];
                }
            }

            var array = bucket[rentalIndex];
            rentals[length]++;
            return array;
        }

        public void Return(T[] array)
        {
            var length = array.Length;
            var maxLengthSupported = buckets.Length - 1;

            if (length > maxLengthSupported)
            {
                return;
            }

            Array.Clear(array, 0, length);
            rentals[length]--;
        }
    }
}