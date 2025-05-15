using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ReflexPlus.Benchmark.Utilities
{
    public abstract class MonoProfiler : MonoBehaviour
    {
        private const int SampleCount = 64;

        [FormerlySerializedAs("_identifier")]
        [SerializeField]
        public string identifier;

        [FormerlySerializedAs("_iterations")]
        [SerializeField, Min(1)]
        public int iterations;

        [FormerlySerializedAs("_resultOutput")]
        [SerializeField]
        public Text resultOutput;

        private readonly RingBuffer<long> samples = new(SampleCount);

        private readonly Dictionary<long, string> stringPool = new();

        protected abstract void Sample();

        private void Update()
        {
            // Sample and measure
            var before = Stopwatch.GetTimestamp();
            Profiler.BeginSample(identifier);
            for (var i = 0; i < iterations; i++)
            {
                Sample();
            }

            Profiler.EndSample();
            var after = Stopwatch.GetTimestamp();
            var elapsedTicks = after - before;
            var elapsedMilliseconds = elapsedTicks / TimeSpan.TicksPerMillisecond;
            samples.Push(elapsedMilliseconds);

            // Present result
            var average = Average(samples);
            if (!stringPool.TryGetValue(average, out var output))
            {
                output = $"{identifier}: {average}";
                stringPool.Add(average, output);
            }

            resultOutput.text = output;
        }

        private static long Average(RingBuffer<long> buffer)
        {
            long total = 0;

            for (var i = 0; i < buffer.Length; i++)
            {
                total += buffer[i];
            }

            return total / buffer.Length;
        }
    }
}