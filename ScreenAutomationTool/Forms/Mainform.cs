using ScreenAutomationTool;
using ScreenAutomationTool.Core;

namespace ScreenAutomation.Forms
{
    public partial class MainForm : Form
    {
        private BotEngine? _bot;

        public MainForm()
        {
            InitializeComponent();
        }

        // ── Start / Stop ────────────────────────────────────────────────

        private async void BtnStart_Click(object? sender, EventArgs e)
        {
            var champions = txtChampions.Text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (champions.Count == 0)
            {
                MessageBox.Show("Enter at least one champion name.",
                    "No Champions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetRunningState(true);

            _bot?.Dispose();
            _bot = new BotEngine(
                champions,
                econFloor: (int)nudEconFloor.Value,
                aggressiveLeveling: chkAggressive.Checked);

            _bot.OnLog += msg => InvokeUI(() => AppendLog(msg));
            _bot.OnStateUpdate += st => InvokeUI(() => UpdateGameStateLabel(st));

            await _bot.RunAsync();

            SetRunningState(false);
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            _bot?.Stop();
        }

        // ── UI helpers ──────────────────────────────────────────────────

        private void SetRunningState(bool running)
        {
            btnStart.Enabled      = !running;
            btnStop.Enabled       = running;
            txtChampions.Enabled  = !running;
            nudEconFloor.Enabled  = !running;
            chkAggressive.Enabled = !running;

            lblStatus.Text      = running ? "Status: Running" : "Status: Idle";
            lblStatus.ForeColor = running ? Color.FromArgb(46, 139, 87) : Color.Gray;
        }

        private void UpdateGameStateLabel(GameState state)
        {
            lblGameState.Text =
                $"Gold: {Fmt(state.Gold)}   Level: {Fmt(state.Level)}   HP: {Fmt(state.Health)}";
        }

        private static string Fmt(int v) => v >= 0 ? v.ToString() : "--";

        private void AppendLog(string message)
        {
            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.ScrollToCaret();
        }

        private void InvokeUI(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        // ── System tray ─────────────────────────────────────────────────

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void TrayShow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        // ── Cleanup ─────────────────────────────────────────────────────

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            _bot?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
