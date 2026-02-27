namespace ScreenAutomation.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtKeywords;
        private Button btnStart;
        private Button btnStop;
        private Button btnClear;
        private RichTextBox rtbLog;
        private NumericUpDown nudRetries;
        private NumericUpDown nudScanInterval;
        private NumericUpDown nudStepDelay;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // --- Keywords ---
            var lblKeywords = new Label();
            lblKeywords.Text = "Keywords (one per line):";
            lblKeywords.Location = new Point(15, 15);
            lblKeywords.AutoSize = true;
            lblKeywords.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtKeywords = new TextBox();
            txtKeywords.Location = new Point(15, 38);
            txtKeywords.Size = new Size(300, 120);
            txtKeywords.Multiline = true;
            txtKeywords.ScrollBars = ScrollBars.Vertical;
            txtKeywords.Font = new Font("Consolas", 10);
            txtKeywords.PlaceholderText = "Enter keywords here...\r\nExample:\r\nOK\r\nNext\r\nSubmit";

            // --- Settings Group ---
            var grpSettings = new GroupBox();
            grpSettings.Text = "Settings";
            grpSettings.Location = new Point(330, 15);
            grpSettings.Size = new Size(340, 143);
            grpSettings.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var lblRetries = new Label();
            lblRetries.Text = "Max Retries:";
            lblRetries.Location = new Point(10, 28);
            lblRetries.AutoSize = true;
            lblRetries.Font = new Font("Segoe UI", 9);

            nudRetries = new NumericUpDown();
            nudRetries.Location = new Point(160, 25);
            nudRetries.Size = new Size(80, 25);
            nudRetries.Minimum = 1;
            nudRetries.Maximum = 100;
            nudRetries.Value = 20;
            nudRetries.Font = new Font("Segoe UI", 9);

            var lblScan = new Label();
            lblScan.Text = "Scan Interval (ms):";
            lblScan.Location = new Point(10, 58);
            lblScan.AutoSize = true;
            lblScan.Font = new Font("Segoe UI", 9);

            nudScanInterval = new NumericUpDown();
            nudScanInterval.Location = new Point(160, 55);
            nudScanInterval.Size = new Size(80, 25);
            nudScanInterval.Minimum = 100;
            nudScanInterval.Maximum = 5000;
            nudScanInterval.Value = 500;
            nudScanInterval.Increment = 100;
            nudScanInterval.Font = new Font("Segoe UI", 9);

            var lblDelay = new Label();
            lblDelay.Text = "Step Delay (ms):";
            lblDelay.Location = new Point(10, 88);
            lblDelay.AutoSize = true;
            lblDelay.Font = new Font("Segoe UI", 9);

            nudStepDelay = new NumericUpDown();
            nudStepDelay.Location = new Point(160, 85);
            nudStepDelay.Size = new Size(80, 25);
            nudStepDelay.Minimum = 100;
            nudStepDelay.Maximum = 10000;
            nudStepDelay.Value = 1000;
            nudStepDelay.Increment = 100;
            nudStepDelay.Font = new Font("Segoe UI", 9);

            grpSettings.Controls.AddRange(new Control[] {
                lblRetries, nudRetries,
                lblScan, nudScanInterval,
                lblDelay, nudStepDelay
            });

            // --- Buttons ---
            btnStart = new Button();
            btnStart.Text = "Start";
            btnStart.Location = new Point(15, 170);
            btnStart.Size = new Size(100, 35);
            btnStart.BackColor = Color.FromArgb(46, 139, 87);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStart.Click += BtnStart_Click;

            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Location = new Point(125, 170);
            btnStop.Size = new Size(100, 35);
            btnStop.BackColor = Color.FromArgb(205, 60, 60);
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStop.Enabled = false;
            btnStop.Click += BtnStop_Click;

            btnClear = new Button();
            btnClear.Text = "Clear Log";
            btnClear.Location = new Point(235, 170);
            btnClear.Size = new Size(80, 35);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 9);
            btnClear.Click += (s, e) => rtbLog.Clear();

            // --- Status ---
            lblStatus = new Label();
            lblStatus.Text = "Status: Idle";
            lblStatus.Location = new Point(330, 175);
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStatus.ForeColor = Color.Gray;

            // --- Log ---
            var lblLog = new Label();
            lblLog.Text = "Log:";
            lblLog.Location = new Point(15, 215);
            lblLog.AutoSize = true;
            lblLog.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            rtbLog = new RichTextBox();
            rtbLog.Location = new Point(15, 238);
            rtbLog.Size = new Size(655, 255);
            rtbLog.ReadOnly = true;
            rtbLog.BackColor = Color.FromArgb(30, 30, 30);
            rtbLog.ForeColor = Color.LightGreen;
            rtbLog.Font = new Font("Consolas", 9.5f);
            rtbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // --- Form ---
            this.Text = "Screen Automation Tool";
            this.ClientSize = new Size(685, 510);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 450);

            this.Controls.AddRange(new Control[] {
                lblKeywords, txtKeywords,
                grpSettings,
                btnStart, btnStop, btnClear, lblStatus,
                lblLog, rtbLog
            });
        }
    }
}