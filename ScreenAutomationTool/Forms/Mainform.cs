using ScreenAutomationTool.Config;
using ScreenAutomationTool.Services;
using ScreenAutomationTool.Interfaces;

namespace ScreenAutomation.Forms
{
    public partial class MainForm : Form
    {
        private IAutomationEngine? _engine;

        public MainForm()
        {
            InitializeComponent();
        }

        private async void BtnStart_Click(object? sender, EventArgs e)
        {
            var keywords = txtKeywords.Text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (keywords.Count == 0)
            {
                MessageBox.Show("Please enter at least one keyword.", "No Keywords",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetRunningState(true);

            var settings = new AutomationSettings
            {
                MaxRetries = (int)nudRetries.Value,
                ScanIntervalMs = (int)nudScanInterval.Value,
                StepDelayMs = (int)nudStepDelay.Value
            };

            _engine?.Dispose();
            _engine = new AutomationEngine(settings);

            _engine.OnLog += message =>
            {
                if (rtbLog.InvokeRequired)
                    rtbLog.Invoke(() => AppendLog(message));
                else
                    AppendLog(message);
            };

            await _engine.RunScript(keywords);

            SetRunningState(false);
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            _engine?.Stop();
        }

        private void SetRunningState(bool running)
        {
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            txtKeywords.Enabled = !running;
            nudRetries.Enabled = !running;
            nudScanInterval.Enabled = !running;
            nudStepDelay.Enabled = !running;

            lblStatus.Text = running ? "Status: Running..." : "Status: Idle";
            lblStatus.ForeColor = running ? Color.FromArgb(46, 139, 87) : Color.Gray;
        }

        private void AppendLog(string message)
        {
            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _engine?.Dispose();
            base.OnFormClosing(e);
        }
    }
}