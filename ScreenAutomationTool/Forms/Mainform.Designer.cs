namespace ScreenAutomation.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtChampions;
        private NumericUpDown nudEconFloor;
        private CheckBox chkAggressive;
        private Button btnStart;
        private Button btnStop;
        private Button btnClear;
        private Label lblStatus;
        private Label lblGameState;
        private RichTextBox rtbLog;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuTray;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // --- Wanted Champions ---
            var lblChampions = new Label();
            lblChampions.Text = "Wanted Champions (one per line):";
            lblChampions.Location = new Point(15, 15);
            lblChampions.AutoSize = true;
            lblChampions.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtChampions = new TextBox();
            txtChampions.Location = new Point(15, 38);
            txtChampions.Size = new Size(300, 120);
            txtChampions.Multiline = true;
            txtChampions.ScrollBars = ScrollBars.Vertical;
            txtChampions.Font = new Font("Consolas", 10);
            txtChampions.PlaceholderText = "Jinx\r\nVi\r\nJayce\r\nCaitlyn";

            // --- Settings Group ---
            var grpSettings = new GroupBox();
            grpSettings.Text = "Settings";
            grpSettings.Location = new Point(330, 15);
            grpSettings.Size = new Size(340, 143);
            grpSettings.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var lblEcon = new Label();
            lblEcon.Text = "Economy Floor:";
            lblEcon.Location = new Point(10, 28);
            lblEcon.AutoSize = true;
            lblEcon.Font = new Font("Segoe UI", 9);

            nudEconFloor = new NumericUpDown();
            nudEconFloor.Location = new Point(160, 25);
            nudEconFloor.Size = new Size(80, 25);
            nudEconFloor.Minimum = 0;
            nudEconFloor.Maximum = 100;
            nudEconFloor.Value = 50;
            nudEconFloor.Increment = 10;
            nudEconFloor.Font = new Font("Segoe UI", 9);

            chkAggressive = new CheckBox();
            chkAggressive.Text = "Aggressive Leveling";
            chkAggressive.Location = new Point(10, 60);
            chkAggressive.AutoSize = true;
            chkAggressive.Font = new Font("Segoe UI", 9);

            grpSettings.Controls.AddRange(new Control[] {
                lblEcon, nudEconFloor, chkAggressive
            });

            // --- Buttons ---
            btnStart = new Button();
            btnStart.Text = "▶ Start";
            btnStart.Location = new Point(15, 170);
            btnStart.Size = new Size(100, 35);
            btnStart.BackColor = Color.FromArgb(46, 139, 87);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStart.Click += BtnStart_Click;

            btnStop = new Button();
            btnStop.Text = "■ Stop";
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
            lblStatus.Location = new Point(330, 170);
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStatus.ForeColor = Color.Gray;

            // --- Live Game State ---
            lblGameState = new Label();
            lblGameState.Text = "Gold: --   Level: --   HP: --";
            lblGameState.Location = new Point(330, 193);
            lblGameState.AutoSize = true;
            lblGameState.Font = new Font("Consolas", 9);

            // --- Log ---
            var lblLog = new Label();
            lblLog.Text = "Log:";
            lblLog.Location = new Point(15, 215);
            lblLog.AutoSize = true;
            lblLog.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            rtbLog = new RichTextBox();
            rtbLog.Location = new Point(15, 238);
            rtbLog.Size = new Size(655, 275);
            rtbLog.ReadOnly = true;
            rtbLog.BackColor = Color.FromArgb(30, 30, 30);
            rtbLog.ForeColor = Color.LightGreen;
            rtbLog.Font = new Font("Consolas", 9.5f);
            rtbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                          | AnchorStyles.Left | AnchorStyles.Right;

            // --- System Tray ---
            contextMenuTray = new ContextMenuStrip(this.components);
            contextMenuTray.Items.Add("Show",  null, (s, e) => TrayShow());
            contextMenuTray.Items.Add("Start", null, (s, e) => BtnStart_Click(s, e));
            contextMenuTray.Items.Add("Stop",  null, (s, e) => BtnStop_Click(s, e));
            contextMenuTray.Items.Add("-");
            contextMenuTray.Items.Add("Exit",  null, (s, e) => { notifyIcon.Visible = false; Application.Exit(); });

            notifyIcon = new NotifyIcon(this.components);
            notifyIcon.Text = "TFT Bot";
            notifyIcon.ContextMenuStrip = contextMenuTray;
            notifyIcon.DoubleClick += (s, e) => TrayShow();
            // Use the application icon; falls back to a default if unavailable.
            notifyIcon.Icon = this.Icon ?? SystemIcons.Application;

            // --- Form ---
            this.Text = "TFT Bot";
            this.ClientSize = new Size(685, 530);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 470);

            this.Controls.AddRange(new Control[] {
                lblChampions, txtChampions,
                grpSettings,
                btnStart, btnStop, btnClear,
                lblStatus, lblGameState,
                lblLog, rtbLog
            });
        }
    }
}
