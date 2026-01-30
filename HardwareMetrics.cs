using System;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace XenoCPUUtilityLegacy
{
    /// <summary>
    /// Hardware metrics provider for CPU load and frequency information.
    /// Ported from XenoCPUUtility for .NET 4.0 compatibility.
    /// </summary>
    public class HardwareMetrics
    {
        public double CpuLoad { get; set; }
        public int CpuFreqMHz { get; set; }
        public double TempC { get; set; }
        public double Voltage { get; set; }
        public double PackagePowerW { get; set; }
        public bool IsValid { get; set; }
    }

    public class HardwareMetricsProvider
    {
        private readonly object sync = new object();
        private PerformanceCounter cpuCounter;
        private int baseClockMHz;
        private bool clockRead = false;

        public HardwareMetricsProvider()
        {
            cpuCounter = TryCreateCounter();
            baseClockMHz = 0;
        }

        public bool TryGetMetrics(out HardwareMetrics metrics)
        {
            metrics = new HardwareMetrics();

            double cpuLoad = SampleCpuLoad();
            if (double.IsNaN(cpuLoad))
            {
                return false;
            }

            if (!clockRead)
            {
                baseClockMHz = ReadBaseClock();
                clockRead = true;
            }

            metrics.CpuLoad = Math.Max(0, Math.Min(100, cpuLoad));
            metrics.CpuFreqMHz = baseClockMHz;
            metrics.TempC = double.NaN;
            metrics.Voltage = double.NaN;
            metrics.PackagePowerW = double.NaN;
            metrics.IsValid = true;
            return true;
        }

        private PerformanceCounter TryCreateCounter()
        {
            try
            {
                return new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            }
            catch
            {
                return null;
            }
        }

        private double SampleCpuLoad()
        {
            if (cpuCounter == null)
            {
                return double.NaN;
            }

            lock (sync)
            {
                try
                {
                    // First call often returns 0, so sample twice when possible.
                    double value = cpuCounter.NextValue();
                    Thread.Sleep(50);
                    value = cpuCounter.NextValue();
                    return value;
                }
                catch
                {
                    return double.NaN;
                }
            }
        }

        private int ReadBaseClock()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor");
                int maxMhz = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    object val = obj["MaxClockSpeed"];
                    if (val != null)
                    {
                        int mhz = Convert.ToInt32(val);
                        maxMhz = Math.Max(maxMhz, mhz);
                    }
                }
                return maxMhz;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetCPUCoreCount()
        {
            return Math.Max(1, Environment.ProcessorCount);
        }

        public static string GetCPUModel()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToString(obj["Name"]);
                }
            }
            catch { }
            return "Unknown";
        }
    }
}
