using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReflexPlus;

namespace ReflexPlusEditor.Tests
{
    internal class SizeSpecificArrayPoolTests
    {
        [Test]
        public void Constructor_InitializesBucketsCorrectly_ReturnsNotNull()
        {
            const int maxLength = 8;
            var pool = new SizeSpecificArrayPool<int>(maxLength);

            for (var i = 0; i <= maxLength; i++)
            {
                Assert.That(pool.Rent(i), Is.Not.Null);
            }
        }

        [Test]
        public void Rent_ReturnsArrayWithCorrectLength_ReturnsExpectedValue()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(5);
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Length, Is.EqualTo(5));
        }

        [Test]
        public void Rent_IncrementsRentalIndex_ReturnsExpectedValue()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var firstArray = pool.Rent(3);
            var secondArray = pool.Rent(3);

            var distinctReferences = new HashSet<int[]>
            {
                firstArray,
                secondArray
            };
            Assert.That(distinctReferences.Count, Is.EqualTo(2));
        }

        [Test]
        public void Return_ClearsArrayOfValueTypes_ReturnsExpectedValue()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(4);
            array[0] = 42;
            array[2] = 42;

            pool.Return(array);
            Assert.That(array.All(n => n == 0), Is.True);
        }

        [Test]
        public void Return_ClearsArrayOfReferenceTypes_ReturnsExpectedValue()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            var array = pool.Rent(4);
            array[0] = new object();
            array[2] = new object();

            pool.Return(array);
            Assert.That(array.All(n => n == null), Is.True);
        }

        [Test]
        public void Rent_AfterReturnReusesArray_ReturnsExpectedValue()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(6);
            pool.Return(array);
            var reusedArray = pool.Rent(6);

            var distinctReferences = new HashSet<int[]>
            {
                array,
                reusedArray
            };
            Assert.That(distinctReferences.Count, Is.EqualTo(1));
        }

        [Test]
        public void Rent_ExpandBucket_DoesNotThrowAnyException()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            Assert.DoesNotThrow(() =>
            {
                for (var i = 0; i < SizeSpecificArrayPool<int>.InitialBucketSize * 2; i++)
                {
                    pool.Rent(1);
                }
            });
        }

        [Test]
        public void Rent_OverMaxLength_DoesNotThrowAnyException()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            Assert.DoesNotThrow(() => { pool.Rent(12); });
        }

        [Test]
        public void Rent_OverMaxLengthDoesReturnUnpooledArray_ReturnsDifferentInstance()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            var array = pool.Rent(12);
            Assert.That(array.Length, Is.EqualTo(12));
            pool.Return(array);
            Assert.That(pool.Rent(12), Is.Not.SameAs(array));
        }
    }
}