using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XenoCPUUtilityLegacy
{
    /// <summary>
    /// Stress engine for CPU stress testing (Heavy Load and Instability Check modes).
    /// Ported from XenoCPUUtility.NativeMethods.StressEngine for .NET 4.0 compatibility.
    /// </summary>
    public class StressEngine
    {
        private readonly object sync = new object();
        private CancellationTokenSource cts;
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private List<Task> workers = new List<Task>();
        private int configuredThreads;
        private string stressMode = "heavy"; // "heavy" or "instability"

        public bool Start(int threadCount, string mode = "heavy")
        {
            lock (sync)
            {
                if (cts != null)
                {
                    return false;
                }

                cts = new CancellationTokenSource();
                pauseEvent.Set();
                configuredThreads = threadCount;
                stressMode = mode;
                workers.Clear();

                for (int i = 0; i < threadCount; i++)
                {
                    workers.Add(Task.Run(() => WorkerLoop(cts.Token), cts.Token));
                }

                return true;
            }
        }

        public void Stop()
        {
            Task[] toWait;

            lock (sync)
            {
                if (cts == null)
                {
                    configuredThreads = 0;
                    return;
                }

                cts.Cancel();
                pauseEvent.Set();
                toWait = workers.ToArray();
                workers.Clear();
                configuredThreads = 0;
            }

            try
            {
                Task.WaitAll(toWait, TimeSpan.FromSeconds(2));
            }
            catch (AggregateException) { }
            catch (OperationCanceledException) { }

            lock (sync)
            {
                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
            }
        }

        public bool Pause()
        {
            lock (sync)
            {
                if (cts == null)
                {
                    return false;
                }

                pauseEvent.Reset();
                return true;
            }
        }

        public bool Resume()
        {
            lock (sync)
            {
                if (cts == null)
                {
                    return false;
                }

                pauseEvent.Set();
                return true;
            }
        }

        public int ActiveThreadCount
        {
            get
            {
                lock (sync)
                {
                    if (cts == null)
                    {
                        return 0;
                    }

                    return pauseEvent.WaitOne(0) ? configuredThreads : 0;
                }
            }
        }

        private void WorkerLoop(CancellationToken token)
        {
            double value = 0.000001d + new Random(Guid.NewGuid().GetHashCode()).NextDouble();
            double increment = 0.0000001d;
            int phase = 0;
            Stopwatch phaseTimer = new Stopwatch();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    pauseEvent.WaitOne();
                    pauseEvent.WaitOne(); // Legacy pattern for older .NET

                    if (stressMode == "instability")
                    {
                        // Rotating 4-phase workload: FP-heavy → Integer → Memory → Mixed
                        if (!phaseTimer.IsRunning)
                        {
                            phaseTimer.Start();
                        }

                        // Switch phase every 2 seconds
                        if (phaseTimer.ElapsedMilliseconds > 2000L)
                        {
                            phase = (phase + 1) % 5; // 5 phases including idle
                            phaseTimer.Restart();
                        }

                        switch (phase)
                        {
                            case 0: // FP-heavy
                                value = Math.Sqrt(value * 1.000001d + increment);
                                value = Math.Sin(value) + 1.000001d;
                                value = Math.Cos(value);
                                break;

                            case 1: // Integer
                                {
                                    int intVal = (int)(value * 1000000);
                                    intVal = (intVal * 7) ^ unchecked((int)0xAAAAAAAA);
                                    intVal = (intVal << 5) | (intVal >> 27);
                                    value = intVal / 1000000.0d;
                                }
                                break;

                            case 2: // Memory pointer-chasing (simulated)
                                {
                                    byte[] scratch = new byte[4096];
                                    for (int i = 0; i < scratch.Length; i++)
                                    {
                                        scratch[i] = (byte)((scratch[i] + (int)value) & 0xFF);
                                    }
                                    value = scratch[0] / 255.0d;
                                }
                                break;

                            case 3: // Mixed bandwidth
                                value = Math.Sqrt(value * 1.000001d + increment);
                                value = Math.Cos(value);
                                break;

                            case 4: // Idle dip (detect voltage droops)
                                Thread.Sleep(50);
                                break;
                        }
                    }
                    else
                    {
                        // Heavy Load mode: continuous sqrt/sin/cos
                        value = Math.Sqrt(value * 1.000001d + increment);
                        value = Math.Sin(value) + 1.000001d;
                        value = Math.Cos(value);
                    }

                    if (value > 4.0d)
                    {
                        value -= 3.75d;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown
            }
            finally
            {
                // Stopwatch doesn't implement IDisposable in .NET Framework 4.x
            }
        }
    }
}
