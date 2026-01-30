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
            cmbPiDigits.SelectedIndex = 4; // Default to 1M
            this.piDigitsBox = cmbPiDigits;

            // PI Benchmark Button (replaces Path Tracer)
            Button btnPiBench = new Button { Text = "Run PI Benchmark", Left = 430, Top = 90, Width = 200, Height = 40 };
            btnPiBench.Click += (s, e) => RunPiBench();

            // Stress Test Buttons
            Button btnStressHeavy = new Button { Text = "Start Heavy Load Stress", Left = 10, Top = 140, Width = 200, Height = 40 };
            btnStressHeavy.Click += (s, e) => StartStressTest("heavy");

            Button btnStressInstability = new Button { Text = "Start Instability Check", Left = 220, Top = 140, Width = 200, Height = 40 };
            btnStressInstability.Click += (s, e) => StartStressTest("instability");

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
            this.Controls.Add(btnPiBench);
            this.Controls.Add(btnStressHeavy);
            this.Controls.Add(btnStressInstability);
            this.Controls.Add(btnPauseResume);
            this.Controls.Add(btnStopStress);
            this.Controls.Add(txtResults);
            this.Controls.Add(lblStatus);

            this.ResumeLayout(false);
        }

        private TextBox resultsBox;
            private TextBox numRunsBox;
            private ComboBox piDigitsBox;
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
            int digits = 1_000_000;
            string digitsLabel = "1M";
            if (piDigitsBox.SelectedIndex >= 0 && piDigitsBox.SelectedIndex < digitCounts.Length)
            {
                digits = digitCounts[piDigitsBox.SelectedIndex];
                digitsLabel = digitOptions[piDigitsBox.SelectedIndex];
            }
            statusLabel.Text = "Running PI benchmark...";
            Application.DoEvents();
            for (int i = 0; i < numRuns; i++)
            {
                // Placeholder: simulate timing
                Stopwatch sw = Stopwatch.StartNew();
                System.Threading.Thread.Sleep(100 + (digits / 1000000) * 50); // Simulate work
                sw.Stop();
                double elapsed = sw.Elapsed.TotalSeconds;
                resultsBox.AppendText(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] PI calculated to {1} digits in ({2:F2}) seconds (Run {3})\r\n", DateTime.Now, digitsLabel, elapsed, i + 1));
                statusLabel.Text = string.Format("PI: {0} digits (Elapsed: {1:F2}s) (Run {2}/{3})", digitsLabel, elapsed, i + 1, numRuns);
                Application.DoEvents();
            }
            statusLabel.Text = string.Format("PI: Completed {0} run(s)", numRuns);
        }

        private void StartStressTest(string mode)
        {
            if (isStressRunning)
            {
                MessageBox.Show("Stress test already running!");
                return;
            }

            int threads = HardwareMetricsProvider.GetCPUCoreCount();
            bool success = stress.Start(threads, mode);

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
