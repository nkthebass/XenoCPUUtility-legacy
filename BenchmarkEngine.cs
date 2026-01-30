using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace XenoCPUUtilityLegacy
{
    /// <summary>
    /// Benchmark engine for single-core and multi-core CPU performance testing.
    /// Ported from XenoCPUUtility.NativeMethods.BenchmarkEngine for .NET 4.0 compatibility.
    /// </summary>
    public class BenchmarkEngine
    {
        private const double TargetSecondsSingle = 10.0;
        private const double TargetSecondsMulti = 6.0;
        private const int OperationsPerBatch = 1024;

        public double RunSingleThread()
        {
            return ExecuteBenchmark(1, TargetSecondsSingle, normalizeForSingle: true);
        }

        public double RunMultiThread()
        {
            int threadCount = Math.Max(1, Environment.ProcessorCount);
            return ExecuteBenchmark(threadCount, TargetSecondsMulti, normalizeForSingle: false);
        }

        private static double ExecuteBenchmark(int threads, double durationSeconds, bool normalizeForSingle)
        {
            long start = Stopwatch.GetTimestamp();
            long durationTicks = (long)(durationSeconds * Stopwatch.Frequency);
            long targetEnd = start + Math.Max(durationTicks, Stopwatch.Frequency / 10);
            long globalIterations = 0;

            if (threads == 1)
            {
                // Single-threaded: avoid Parallel.For overhead
                double x = 1.0d;
                double y = 1.0d;

                while (true)
                {
                    for (int i = 0; i < OperationsPerBatch; i++)
                    {
                        x = Math.Sqrt(x * 1.0000005d + 0.0000008d);
                        y = Math.Cos(x) * Math.Sin(y) + 1.0000002d;
                    }

                    globalIterations += OperationsPerBatch;

                    if (Stopwatch.GetTimestamp() >= targetEnd)
                    {
                        break;
                    }
                }
            }
            else
            {
                // Multi-threaded: use Parallel.For
                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = threads
                };

                Parallel.For(0, threads, options, () => 0L, (index, state, localIterations) =>
                {
                    double x = 1.0d + index * 0.15d;
                    double y = 1.0d + index * 0.07d;

                    while (true)
                    {
                        for (int i = 0; i < OperationsPerBatch; i++)
                        {
                            x = Math.Sqrt(x * 1.0000005d + 0.0000008d);
                            y = Math.Cos(x) * Math.Sin(y) + 1.0000002d;
                        }

                        localIterations += OperationsPerBatch;

                        if (Stopwatch.GetTimestamp() >= targetEnd)
                        {
                            break;
                        }
                    }

                    return localIterations;
                }, localIterations => 
                {
                    lock (typeof(long))
                    {
                        globalIterations += localIterations;
                    }
                });
            }

            double elapsedSeconds = Math.Max((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency, 1e-5d);
            double operationsPerSecond = globalIterations / elapsedSeconds;

            double normalization = normalizeForSingle ? 1_456_000d : 230_000d;
            double score = operationsPerSecond / normalization;

            if (!normalizeForSingle)
            {
                score /= 4.67; // User calibration: divide multi-core score, no thread scaling
                score *= 2.676; // User calibration: multiply multi-core score
            }
            else
            {
                score *= 2.875; // User calibration: multiply single-core score
            }

            return Math.Round(Math.Max(score, 0d), 1);
        }
    }
}
