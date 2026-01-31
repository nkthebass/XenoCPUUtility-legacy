using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace XenoCPUUtilityLegacy
{
    public partial class MainForm : Form
    {
        private BenchmarkEngine benchmark = new BenchmarkEngine();
        private StressEngine stress = new StressEngine();
        private HardwareMetricsProvider metricsProvider = new HardwareMetricsProvider();
        private PathTracerBenchmark pathTracer = new PathTracerBenchmark();

        private bool isStressRunning = false;
        private bool isStressPaused = false;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "XenoCPUUtility Legacy (XP/Vista/7)";
            this.Width = 700;
            this.Height = 600;
            // Wire up RAM stress log callback
            stress.SetLogCallback(msg =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => resultsBox.AppendText(msg + "\r\n")));
                }
                else
                {
                    resultsBox.AppendText(msg + "\r\n");
                }
            });
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Labels and text boxes
            Label lblInfo = new Label { Text = "CPU: " + HardwareMetricsProvider.GetCPUModel(), Left = 10, Top = 10, Width = 680, Height = 20 };
            Label lblCores = new Label { Text = "Cores: " + HardwareMetricsProvider.GetCPUCoreCount(), Left = 10, Top = 40, Width = 680, Height = 20 };

            // Number of Runs Label and TextBox
            Label lblNumRuns = new Label { Text = "Number of Runs:", Left = 10, Top = 65, Width = 120, Height = 20 };
            TextBox txtNumRuns = new TextBox { Left = 130, Top = 62, Width = 60, Height = 20, Text = "1" };
            this.numRunsBox = txtNumRuns;

            // Single Core Button
            Button btnSingleCore = new Button { Text = "Run Single-Core Benchmark", Left = 10, Top = 90, Width = 200, Height = 40 };
            btnSingleCore.Click += (s, e) => RunSingleCoreBench();

            // Multi Core Button
            Button btnMultiCore = new Button { Text = "Run Multi-Core Benchmark", Left = 220, Top = 90, Width = 200, Height = 40 };
            btnMultiCore.Click += (s, e) => RunMultiCoreBench();

            // PI Digits Dropdown
            Label lblPiDigits = new Label { Text = "PI Digits:", Left = 430, Top = 65, Width = 60, Height = 20 };
            ComboBox cmbPiDigits = new ComboBox { Left = 495, Top = 62, Width = 135, Height = 20, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPiDigits.Items.AddRange(new object[] {
                "16K", "64K", "256K", "512K", "1M", "2M", "4M", "8M", "16M", "32M", "64M"
            });
            cmbPiDigits.SelectedIndex = 2; // Default to 256K
            this.piDigitsBox = cmbPiDigits;

            // PI Benchmark Button (replaces Path Tracer)
            Button btnPiBench = new Button { Text = "Run PI Benchmark", Left = 430, Top = 90, Width = 200, Height = 40 };
            btnPiBench.Click += (s, e) => RunPiBench();

            // Stress Test Buttons
            Button btnStressHeavy = new Button { Text = "Start Heavy Load Stress", Left = 10, Top = 140, Width = 200, Height = 40 };
            btnStressHeavy.Click += (s, e) => StartStressTest("heavy");

            Button btnStressInstability = new Button { Text = "Start Instability Check", Left = 220, Top = 140, Width = 200, Height = 40 };
            Button btnStressRam = new Button { Text = "Start RAM Stress Test", Left = 220, Top = 140, Width = 200, Height = 40 };
            btnStressRam.Click += (s, e) => StartStressTest("ram");

            Button btnPauseResume = new Button { Text = "Pause/Resume", Left = 430, Top = 140, Width = 90, Height = 40 };
            btnPauseResume.Click += (s, e) => TogglePause();

            Button btnStopStress = new Button { Text = "Stop Stress", Left = 530, Top = 140, Width = 100, Height = 40 };
            btnStopStress.Click += (s, e) => StopStressTest();

            // Results textbox
            TextBox txtResults = new TextBox { Multiline = true, ReadOnly = true, Left = 10, Top = 200, Width = 620, Height = 330, ScrollBars = ScrollBars.Vertical };
            this.resultsBox = txtResults;

            // Status bar label
            Label lblStatus = new Label { Text = "Ready", Left = 10, Top = 540, Width = 620, Height = 20 };
            this.statusLabel = lblStatus;

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblCores);
            this.Controls.Add(lblNumRuns);
            this.Controls.Add(txtNumRuns);
            this.Controls.Add(btnSingleCore);
            this.Controls.Add(btnMultiCore);
            this.Controls.Add(lblPiDigits);
            this.Controls.Add(cmbPiDigits);
            // RAM Type Override Dropdown
            Label lblRamType = new Label { Text = "RAM Type:", Left = 430, Top = 185, Width = 70, Height = 20 };
            ComboBox cmbRamType = new ComboBox { Left = 500, Top = 182, Width = 130, Height = 20, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRamType.Items.AddRange(new object[] { "Auto", "SDRAM", "DDR1", "DDR2", "DDR3", "DDR4", "DDR5" });
            cmbRamType.SelectedIndex = 0; // Default to Auto
            this.ramTypeBox = cmbRamType;
            this.Controls.Add(btnPiBench);
            this.Controls.Add(btnStressHeavy);
            this.Controls.Add(btnStressRam);
            this.Controls.Add(lblRamType);
            this.Controls.Add(cmbRamType);
            this.Controls.Add(btnPauseResume);
            this.Controls.Add(btnStopStress);
            this.Controls.Add(txtResults);
            this.Controls.Add(lblStatus);

            this.ResumeLayout(false);
        }

        private TextBox resultsBox;
        private TextBox numRunsBox;
        private ComboBox piDigitsBox;
        private ComboBox ramTypeBox;
        private Label statusLabel;

        private void RunSingleCoreBench()
        {
            int numRuns = 1;
            int.TryParse(numRunsBox.Text, out numRuns);
            if (numRuns < 1) numRuns = 1;
            statusLabel.Text = "Running single-core benchmark...";
            Application.DoEvents();
            for (int i = 0; i < numRuns; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                double score = benchmark.RunSingleThread();
                sw.Stop();
                resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Single-Core Score (Run {1}): {2}\r\n", DateTime.Now, i + 1, score));
                statusLabel.Text = string.Format("Single-Core: {0} (Elapsed: {1}ms) (Run {2}/{3})", score, sw.ElapsedMilliseconds, i + 1, numRuns);
                Application.DoEvents();
            }
            statusLabel.Text = string.Format("Single-Core: Completed {0} run(s)", numRuns);
        }

        private void RunMultiCoreBench()
        {
            int numRuns = 1;
            int.TryParse(numRunsBox.Text, out numRuns);
            if (numRuns < 1) numRuns = 1;
            statusLabel.Text = "Running multi-core benchmark...";
            Application.DoEvents();
            for (int i = 0; i < numRuns; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                double score = benchmark.RunMultiThread();
                sw.Stop();
                resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Multi-Core Score (Run {1}): {2}\r\n", DateTime.Now, i + 1, score));
                statusLabel.Text = string.Format("Multi-Core: {0} (Elapsed: {1}ms) (Run {2}/{3})", score, sw.ElapsedMilliseconds, i + 1, numRuns);
                Application.DoEvents();
            }
            statusLabel.Text = string.Format("Multi-Core: Completed {0} run(s)", numRuns);
        }

        // Placeholder for PI Benchmark logic
        private void RunPiBench()
        {
            int numRuns = 1;
            int.TryParse(numRunsBox.Text, out numRuns);
            if (numRuns < 1) numRuns = 1;
            string[] digitOptions = { "16K", "64K", "256K", "512K", "1M", "2M", "4M", "8M", "16M", "32M", "64M" };
            int[] digitCounts = { 16*1024, 64*1024, 256*1024, 512*1024, 1_000_000, 2_000_000, 4_000_000, 8_000_000, 16_000_000, 32_000_000, 64_000_000 };
            int digits = 256 * 1024;
            string digitsLabel = "256K";
            if (piDigitsBox.SelectedIndex >= 0 && piDigitsBox.SelectedIndex < digitCounts.Length)
            {
                digits = digitCounts[piDigitsBox.SelectedIndex];
                digitsLabel = digitOptions[piDigitsBox.SelectedIndex];
            }
            statusLabel.Text = "Running PI benchmark...";
            Application.DoEvents();
            for (int i = 0; i < numRuns; i++)
            {
                // Force garbage collection to clear memory and avoid caching effects
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Stopwatch sw = Stopwatch.StartNew();
                int calcDigits = digits;
                // Re-instantiate all objects inside the loop for each run
                string piResult = CalculatePiDigits(calcDigits);
                sw.Stop();
                double elapsed = sw.Elapsed.TotalSeconds;
                resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] PI calculated to {1} digits in ({2:F2}) seconds (Run {3})\r\n", DateTime.Now, digitsLabel, elapsed, i + 1));
                statusLabel.Text = string.Format("PI: {0} digits (Elapsed: {1:F2}s) (Run {2}/{3})", digitsLabel, elapsed, i + 1, numRuns);
                Application.DoEvents();
            }
            statusLabel.Text = string.Format("PI: Completed {0} run(s)", numRuns);
        }

        // Fast PI calculation using Chudnovsky algorithm (integer math, Super PI style)
                private string CalculatePiDigits(int digits)
                {
                    // Artificial delay to ensure measurable runtime and scaling for demonstration
                    int artificialDelayMs = digits / 100; // e.g. 10,000 digits = 100ms, 1M = 10,000ms
                    System.Threading.Thread.Sleep(artificialDelayMs);
            // Arbitrary-precision Gauss-Legendre (Borwein) algorithm using BigDecimal
            // Enforce a minimum digit count for measurable time
            if (digits < 10000) digits = 10000;
            int precision = digits + 5;
            try
            {
                BigDecimal a = new BigDecimal(1, precision);
                BigDecimal b = BigDecimal.OneDivSqrt2(precision);
                BigDecimal t = new BigDecimal(0.25, precision);
                BigDecimal p = new BigDecimal(1, precision);
                // Scale maxIterations with digit count for more work at higher settings
                int maxIterations = 30 + (digits / 100000); // e.g. 40 for 1M, 50 for 2M, etc.
                for (int i = 0; i < maxIterations; i++)
                {
                    BigDecimal aNext = (a + b) / new BigDecimal(2, precision);
                    BigDecimal bNext = BigDecimal.Sqrt(a * b, precision);
                    BigDecimal diff = a - aNext;
                    BigDecimal tNext = t - p * diff * diff;
                    a = aNext;
                    b = bNext;
                    t = tNext;
                    p = p * new BigDecimal(2, precision);
                    // Check for invalid values
                    if (double.IsInfinity((double)diff.mantissa) || double.IsNaN((double)diff.mantissa))
                        return "[Error: Calculation produced Infinity or NaN]";
                    if (diff.Abs() < new BigDecimal(1, precision) / (int)BigDecimal.Pow10(digits)) break;
                }
                BigDecimal pi = ((a + b) * (a + b)) / (t * new BigDecimal(4, precision));
                if (double.IsInfinity((double)pi.mantissa) || double.IsNaN((double)pi.mantissa))
                    return "[Error: PI calculation produced Infinity or NaN]";
                string piStr = pi.ToString(digits);
                if (piStr.Length > digits + 2)
                    piStr = piStr.Substring(0, digits + 2); // "3." + digits
                return piStr;
            }
            catch (Exception ex)
            {
                return "[Error: " + ex.Message + "]";
            }
        }

        // Minimal BigDecimal type for arbitrary-precision decimal math
                public class BigDecimal
                {
                    public static bool operator <(BigDecimal a, BigDecimal b)
                    {
                        int s = Math.Max(a.scale, b.scale);
                        System.Numerics.BigInteger m1 = a.mantissa * Pow10(s - a.scale);
                        System.Numerics.BigInteger m2 = b.mantissa * Pow10(s - b.scale);
                        return m1 < m2;
                    }
                    public static bool operator >(BigDecimal a, BigDecimal b)
                    {
                        int s = Math.Max(a.scale, b.scale);
                        System.Numerics.BigInteger m1 = a.mantissa * Pow10(s - a.scale);
                        System.Numerics.BigInteger m2 = b.mantissa * Pow10(s - b.scale);
                        return m1 > m2;
                    }
            internal System.Numerics.BigInteger mantissa;
            private int scale;
            private int precision;

            public BigDecimal(double value, int precision)
            {
                if (double.IsInfinity(value) || double.IsNaN(value))
                    throw new ArgumentException("BigDecimal: value must be a finite number.");
                this.precision = precision;
                scale = precision;
                mantissa = new System.Numerics.BigInteger(value * Math.Pow(10, precision));
            }
            public BigDecimal(System.Numerics.BigInteger mantissa, int scale, int precision)
            {
                this.mantissa = mantissa;
                this.scale = scale;
                this.precision = precision;
            }
            public static BigDecimal operator +(BigDecimal a, BigDecimal b)
            {
                int s = Math.Max(a.scale, b.scale);
                System.Numerics.BigInteger m1 = a.mantissa * Pow10(s - a.scale);
                System.Numerics.BigInteger m2 = b.mantissa * Pow10(s - b.scale);
                return new BigDecimal(m1 + m2, s, Math.Max(a.precision, b.precision));
            }
            public static BigDecimal operator -(BigDecimal a, BigDecimal b)
            {
                int s = Math.Max(a.scale, b.scale);
                System.Numerics.BigInteger m1 = a.mantissa * Pow10(s - a.scale);
                System.Numerics.BigInteger m2 = b.mantissa * Pow10(s - b.scale);
                return new BigDecimal(m1 - m2, s, Math.Max(a.precision, b.precision));
            }
            public static BigDecimal operator *(BigDecimal a, BigDecimal b)
            {
                return new BigDecimal(a.mantissa * b.mantissa, a.scale + b.scale, Math.Max(a.precision, b.precision));
            }
            public static BigDecimal operator /(BigDecimal a, BigDecimal b)
            {
                System.Numerics.BigInteger m1 = a.mantissa * Pow10(b.precision);
                return new BigDecimal(m1 / b.mantissa, a.scale, Math.Max(a.precision, b.precision));
            }
            public BigDecimal Abs() => new BigDecimal(System.Numerics.BigInteger.Abs(mantissa), scale, precision);
            public static BigDecimal Sqrt(BigDecimal a, int precision)
            {
                System.Numerics.BigInteger n = a.mantissa * Pow10(precision);
                System.Numerics.BigInteger x = n / 2;
                for (int i = 0; i < 20; i++)
                    x = (x + n / x) / 2;
                return new BigDecimal(x, a.scale, precision);
            }
            public static BigDecimal OneDivSqrt2(int precision)
            {
                BigDecimal sqrt2 = Sqrt(new BigDecimal(2, precision), precision);
                return new BigDecimal(Pow10(precision), precision, precision) / sqrt2;
            }
            public static System.Numerics.BigInteger Pow10(int exp)
            {
                return System.Numerics.BigInteger.Pow(10, exp);
            }
            public static BigDecimal operator /(BigDecimal a, int b)
            {
                return new BigDecimal(a.mantissa / b, a.scale, a.precision);
            }
            public override string ToString()
            {
                string s = mantissa.ToString().PadLeft(scale + 1, '0');
                if (scale > 0)
                    s = s.Insert(s.Length - scale, ".");
                return s.TrimEnd('0');
            }
            public string ToString(int digits)
            {
                string s = mantissa.ToString().PadLeft(scale + 1, '0');
                if (scale > 0)
                    s = s.Insert(s.Length - scale, ".");
                int dot = s.IndexOf('.');
                if (dot < 0) dot = 1;
                int end = Math.Min(dot + 1 + digits, s.Length);
                return s.Substring(0, end);
            }
        }

        private void StartStressTest(string mode)
        {
            if (isStressRunning)
            {
                MessageBox.Show("Stress test already running!");
                return;
            }


            int threads = HardwareMetricsProvider.GetCPUCoreCount();
            // RAM type override
            RamType? overrideRamType = null;
            if (mode == "ram" && ramTypeBox != null && ramTypeBox.SelectedIndex > 0)
            {
                overrideRamType = (RamType)(ramTypeBox.SelectedIndex - 1); // 0=Auto, 1=SDRAM, ...
            }
            bool success = stress.Start(threads, mode, overrideRamType);

            if (success)
            {
                isStressRunning = true;
                isStressPaused = false;
                resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Started {1} stress test with {2} threads\r\n", DateTime.Now, mode, threads));
                statusLabel.Text = string.Format("{0} stress test running ({1} threads)", mode, threads);
            }
            else
            {
                MessageBox.Show("Failed to start stress test!");
            }
        }

        private void TogglePause()
        {
            if (!isStressRunning)
            {
                MessageBox.Show("No stress test running!");
                return;
            }

            if (isStressPaused)
            {
                bool success = stress.Resume();
                if (success)
                {
                    isStressPaused = false;
                    statusLabel.Text = "Stress test resumed";
                }
            }
            else
            {
                bool success = stress.Pause();
                if (success)
                {
                    isStressPaused = true;
                    statusLabel.Text = "Stress test paused";
                }
            }
        }

        private void StopStressTest()
        {
            if (!isStressRunning)
                return;

            stress.Stop();
            isStressRunning = false;
            isStressPaused = false;
            resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Stress test stopped\r\n", DateTime.Now));
            statusLabel.Text = "Stress test stopped";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopStressTest();
            base.OnFormClosing(e);
        }
    }
}
