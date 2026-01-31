using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using System.Management;
namespace XenoCPUUtilityLegacy
{

    public class StressEngine
    {
        // RAM profile for tuning
        private struct RamProfile
        {
            public int Stride;
            public bool UseWalkingBits;
            public bool UseRandom;
        }
        private static RamProfile GetProfile(RamType type)
        {
            switch (type)
            {
                case RamType.SDRAM:
                    return new RamProfile { Stride = 4, UseWalkingBits = true, UseRandom = false };
                case RamType.DDR1:
                    return new RamProfile { Stride = 8, UseWalkingBits = true, UseRandom = false };
                case RamType.DDR2:
                    return new RamProfile { Stride = 64, UseWalkingBits = false, UseRandom = true };
                case RamType.DDR3:
                    return new RamProfile { Stride = 128, UseWalkingBits = false, UseRandom = true };
                case RamType.DDR4:
                    return new RamProfile { Stride = 256, UseWalkingBits = false, UseRandom = true };
                case RamType.DDR5:
                    return new RamProfile { Stride = 512, UseWalkingBits = false, UseRandom = true };
                default:
                    return new RamProfile { Stride = 128, UseWalkingBits = false, UseRandom = true };
            }
        }

        // Delegate for logging to UI
        public delegate void LogCallback(string msg);
        private LogCallback logCallback;

        public void SetLogCallback(LogCallback cb)
        {
            logCallback = cb;
        }

            // Robust RAM type detection using SMBIOS, speed, and majority vote
            private static RamType DetectRamTypeReliable(Action<string> log = null)
            {
                var detected = new List<RamType>();
                try
                {
                    using (var searcher = new System.Management.ManagementObjectSearcher(
                        "SELECT SMBIOSMemoryType, MemoryType, ConfiguredClockSpeed, Speed, Voltage FROM Win32_PhysicalMemory"))
                    {
                        foreach (System.Management.ManagementObject obj in searcher.Get())
                        {
                            ushort smbios = obj["SMBIOSMemoryType"] != null ? (ushort)obj["SMBIOSMemoryType"] : (ushort)0;
                            ushort legacy = obj["MemoryType"] != null ? (ushort)obj["MemoryType"] : (ushort)0;
                            uint speed = 0;
                            if (obj["ConfiguredClockSpeed"] != null)
                                speed = (uint)obj["ConfiguredClockSpeed"];
                            else if (obj["Speed"] != null)
                                speed = (uint)obj["Speed"];
                            string logline = $"DIMM: SMBIOS={smbios}, Legacy={legacy}, Speed={speed}";
                            RamType t = RamType.Unknown;
                            switch (smbios)
                            {
                                case 14: t = RamType.SDRAM; break;
                                case 18: t = RamType.DDR1; break;
                                case 19: t = RamType.DDR2; break;
                                case 24: t = RamType.DDR3; break;
                                case 26: t = RamType.DDR4; break;
                                case 34: t = RamType.DDR5; break;
                            }
                            if (t == RamType.Unknown)
                            {
                                switch (legacy)
                                {
                                    case 5: t = RamType.SDRAM; break;
                                    case 8: t = RamType.DDR1; break;
                                    case 9: t = RamType.DDR2; break;
                                    case 11: t = RamType.DDR3; break;
                                }
                            }
                            // Frequency-based inference
                            if (t == RamType.Unknown && speed > 0)
                            {
                                if (speed <= 133) t = RamType.SDRAM;
                                else if (speed <= 266) t = RamType.DDR1;
                                else if (speed <= 533) t = RamType.DDR2;
                                else if (speed <= 1866) t = RamType.DDR3;
                                else t = RamType.DDR4; // >=2133 MHz
                            }
                            logline += $", Detected={t}";
                            log?.Invoke(logline);
                            detected.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log?.Invoke("RAM detection error: " + ex.Message);
                }
                // Majority vote
                if (detected.Count == 0)
                    return RamType.DDR3;
                int sdram = 0, ddr1 = 0, ddr2 = 0, ddr3 = 0, ddr4 = 0, ddr5 = 0;
                foreach (var t in detected)
                {
                    switch (t)
                    {
                        case RamType.SDRAM: sdram++; break;
                        case RamType.DDR1: ddr1++; break;
                        case RamType.DDR2: ddr2++; break;
                        case RamType.DDR3: ddr3++; break;
                        case RamType.DDR4: ddr4++; break;
                        case RamType.DDR5: ddr5++; break;
                    }
                }
                if (ddr5 > 0) return RamType.DDR5;
                if (ddr4 > 0) return RamType.DDR4;
                if (ddr3 >= ddr2 && ddr3 >= ddr1 && ddr3 >= sdram) return RamType.DDR3;
                if (ddr2 >= ddr1 && ddr2 >= sdram) return RamType.DDR2;
                if (ddr1 >= sdram) return RamType.DDR1;
                return RamType.SDRAM;
            }

        private readonly object sync = new object();
        private CancellationTokenSource cts;
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private List<Task> workers = new List<Task>();
        private int configuredThreads;
        private string stressMode = "heavy"; // "heavy" or "instability"

        public bool Start(int threadCount, string mode = "heavy", RamType? overrideRamType = null)
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
                this.overrideRamType = overrideRamType;

                for (int i = 0; i < threadCount; i++)
                {
                    workers.Add(Task.Factory.StartNew(() => WorkerLoop(cts.Token), cts.Token));
                }

                return true;
            }
        }

        private RamType? overrideRamType = null;

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
            if (stressMode == "ram")
            {
                RunRamStress(token);
                return;
            }

            // ...existing code for CPU stress...
            double a = 0.000123d;
            double b = 1.000321d;
            double c = 0.999777d;
            double d = 1.000999d;
            int ia = 0x12345678;
            int ib = unchecked((int)0x9ABCDEF0);
            int ic = 0x0F0F0F0F;
            int id = 0x33333333;
            byte[] mem = new byte[32 * 1024];
            int memIndex = 0;
            int phase = 0;
            Stopwatch phaseTimer = Stopwatch.StartNew();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    pauseEvent.WaitOne();
                    pauseEvent.WaitOne();
                    if (phaseTimer.ElapsedMilliseconds > 2000)
                    {
                        phase = (phase + 1) % 5;
                        phaseTimer.Restart();
                    }
                    switch (phase)
                    {
                        case 0:
                            for (int i = 0; i < 64; i++)
                            {
                                a = Math.Sqrt(a * 1.000001d + 0.0000001d);
                                b = Math.Sqrt(b * 0.999999d + 0.0000002d);
                                c = Math.Sin(c + a);
                                d = Math.Cos(d + b);
                                a += d * 0.000001d;
                                b += c * 0.000001d;
                            }
                            break;
                        case 1:
                            for (int i = 0; i < 256; i++)
                            {
                                ia = (ia * 1664525) ^ ib;
                                ib = (ib << 5) | (ib >> 27);
                                ic += ia ^ id;
                                id = unchecked(id * 1103515245 + 12345);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < mem.Length; i += 64)
                            {
                                mem[memIndex] ^= (byte)ia;
                                memIndex = (memIndex + 64) & (mem.Length - 1);
                            }
                            break;
                        case 3:
                            a = Math.Sqrt(a + b);
                            b = Math.Sin(b + c);
                            c = Math.Cos(c + d);
                            d = Math.Sqrt(d + a);
                            ia ^= (int)a;
                            ib ^= (int)b;
                            ic ^= (int)c;
                            id ^= (int)d;
                            break;
                        case 4:
                            Thread.Sleep(50);
                            break;
                    }
                    if (a > 10.0d) a -= 9.5d;
                    if (b > 10.0d) b -= 9.5d;
                    if (c > 10.0d) c -= 9.5d;
                    if (d > 10.0d) d -= 9.5d;
                }
            }
            catch (OperationCanceledException) { }
        }

        // RAM stress logic (auto-tuning)
        private void RunRamStress(CancellationToken token)
        {
            // Use robust detection and log all DIMM info
            RamType ramType = overrideRamType ?? DetectRamTypeReliable(msg => logCallback?.Invoke(msg));
            RamProfile profile = GetProfile(ramType);
            switch (ramType)
            {
                case RamType.DDR4:
                    logCallback?.Invoke("DDR4 tuning: large stride, random patterns, optimized for high bandwidth");
                    break;
                case RamType.DDR5:
                    logCallback?.Invoke("DDR5 tuning: extra-large stride, random patterns, optimized for extreme bandwidth");
                    break;
            }
            logCallback?.Invoke("Using stride: " + profile.Stride + " bytes");

            // Get total system memory (in bytes)
            ulong totalMem = 0;
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMem = (ulong)(obj["TotalPhysicalMemory"] ?? 0);
                }
            }
            catch { }
            if (totalMem == 0) totalMem = 512UL * 1024 * 1024; // fallback 512MB


            // Use multiple buffers to keep ~25% of RAM allocated at all times
            ulong targetMem = totalMem / 4; // 25%
            if (targetMem < 32UL * 1024 * 1024) targetMem = 32UL * 1024 * 1024; // minimum 32MB
            // For 32-bit process, cap at 2GB
            ulong maxAlloc = (Environment.Is64BitProcess ? targetMem : Math.Min(targetMem, 2UL * 1024 * 1024 * 1024));
            // Use a single buffer if possible, else split into multiple buffers of reasonable size
            int singleBufferSize = (int)Math.Min(maxAlloc, 512UL * 1024 * 1024); // max 512MB per buffer
            int numFullBuffers = (int)(maxAlloc / (ulong)singleBufferSize);
            ulong remainder = maxAlloc % (ulong)singleBufferSize;
            ulong totalAlloc = (ulong)numFullBuffers * (ulong)singleBufferSize + remainder;
            logCallback?.Invoke($"Allocating {numFullBuffers} x {singleBufferSize / (1024 * 1024)}MB" + (remainder > 0 ? $" + 1 x {remainder / (1024 * 1024)}MB" : "") + $" = {totalAlloc / (1024 * 1024)}MB for RAM stress");

            List<byte[]> buffers = new List<byte[]>();
            for (int i = 0; i < numFullBuffers; i++)
            {
                buffers.Add(new byte[singleBufferSize]);
            }
            if (remainder > 0)
            {
                buffers.Add(new byte[(int)remainder]);
            }
            Random rng = new Random(Environment.TickCount);

            // LCG for random pattern
            byte LcgByte(int i)
            {
                uint x = (uint)(i * 1664525 + 1013904223);
                return (byte)(x & 0xFF);
            }

            Func<int, byte>[] patterns;
            if (profile.UseWalkingBits)
            {
                patterns = new Func<int, byte>[]
                {
                    i => (byte)(1 << (i & 7)),
                    i => (byte)~(1 << (i & 7)),
                    i => 0xAA,
                    i => 0x55
                };
            }
            else
            {
                patterns = new Func<int, byte>[]
                {
                    i => 0x00,
                    i => 0xFF,
                    i => (byte)(i & 0xFF),
                    i => profile.UseRandom ? LcgByte(i) : (byte)0xAA
                };
            }

            int patternCount = patterns.Length;
            int stride = profile.Stride;
            int errors = 0;
            int passes = 0;
            try
            {
                int rotate = 0;
                while (!token.IsCancellationRequested)
                {
                    pauseEvent.WaitOne();
                    pauseEvent.WaitOne();

                    // Rotate: move to next buffer index (circular)
                    int bufferIndex = passes % buffers.Count;

                    // Only stress one buffer per pass (rotate through all)
                    var buffer = buffers[bufferIndex];
                    for (int p = 0; p < patternCount; p++)
                    {
                        // Write pattern
                        for (int i = 0; i < buffer.Length; i += stride)
                        {
                            buffer[i] = patterns[p](i);
                        }
                        // Verify pattern
                        for (int i = 0; i < buffer.Length; i += stride)
                        {
                            byte expected = patterns[p](i);
                            byte actual = buffer[i];
                            if (actual != expected)
                            {
                                errors++;
                                if (errors < 100)
                                    logCallback?.Invoke($"RAM ERROR: offset 0x{i:X} expected 0x{expected:X2} got 0x{actual:X2}");
                            }
                        }
                    }
                    // Throttle after each buffer to reduce CPU usage
                    Thread.Sleep(30);
                    passes++;
                    if (passes % 10 == 0)
                        logCallback?.Invoke($"RAM stress: {passes} passes, {errors} errors");
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                // Free all buffers and force GC
                buffers.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
