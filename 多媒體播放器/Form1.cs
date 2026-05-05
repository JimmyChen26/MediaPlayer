using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace 多媒體播放器
{
    public partial class frmMediaPlayer : Form
    {
        private Panel infoPanel;
        private Label lblInstruction;
        private Label lblFileName;

        public frmMediaPlayer()
        {
            InitializeComponent();

            CreateInfoPanel();
            SetTheme();
            ArrangeLayout();

            this.Resize += frmMediaPlayer_Resize;
        }

        // 設定整體配色
        private void SetTheme()
        {
            // 表單背景色
            this.BackColor = Color.FromArgb(232, 240, 248);

            // 操作說明區背景色
            infoPanel.BackColor = Color.FromArgb(250, 252, 255);

            // 底部按鈕區背景色
            palButton.BackColor = Color.FromArgb(216, 231, 246);

            // 按鈕顏色
            StyleButton(btnBrowser, Color.FromArgb(91, 155, 213));   // 瀏覽：藍色
            StyleButton(btnPlay, Color.FromArgb(112, 173, 71));      // 播放：綠色
            StyleButton(btnPause, Color.FromArgb(255, 192, 0));      // 暫停：黃色
            StyleButton(btnStop, Color.FromArgb(192, 80, 77));       // 停止：紅色
        }

        // 統一設定按鈕樣式
        private void StyleButton(Button btn, Color backColor)
        {
            btn.Size = new Size(110, 55);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("微軟正黑體", 14F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        // 建立操作說明區
        private void CreateInfoPanel()
        {
            infoPanel = new Panel();
            infoPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(infoPanel);

            lblInstruction = new Label();
            lblInstruction.Text =
                "操作說明：\n" +
                "1. 先按「瀏覽」選擇影片。\n" +
                "2. 再按「播放」開始播放。\n" +
                "3. 播放中可按「暫停」，按「停止」可停止影片。";

            lblInstruction.Font = new Font("微軟正黑體", 10F, FontStyle.Regular);
            lblInstruction.ForeColor = Color.FromArgb(50, 50, 50);
            lblInstruction.TextAlign = ContentAlignment.TopLeft;
            lblInstruction.AutoSize = false;
            infoPanel.Controls.Add(lblInstruction);

            lblFileName = new Label();
            lblFileName.Text = "目前檔案：尚未選擇影片";
            lblFileName.Font = new Font("微軟正黑體", 10F, FontStyle.Bold);
            lblFileName.ForeColor = Color.FromArgb(80, 80, 80);
            lblFileName.TextAlign = ContentAlignment.MiddleLeft;
            lblFileName.AutoSize = false;
            infoPanel.Controls.Add(lblFileName);
        }

        // 自動排版
        private void ArrangeLayout()
        {
            int margin = 30;
            int infoHeight = 145;
            int buttonPanelHeight = 100;

            // ===== 操作說明區 =====
            infoPanel.Left = margin;
            infoPanel.Top = 25;
            infoPanel.Width = this.ClientSize.Width - margin * 2;
            infoPanel.Height = infoHeight;

            lblInstruction.Left = 20;
            lblInstruction.Top = 12;
            lblInstruction.Width = infoPanel.Width - 40;
            lblInstruction.Height = 90;

            lblFileName.Left = 20;
            lblFileName.Top = 105;
            lblFileName.Width = infoPanel.Width - 40;
            lblFileName.Height = 30;

            // ===== 底部按鈕區 =====
            palButton.Left = 0;
            palButton.Top = this.ClientSize.Height - buttonPanelHeight;
            palButton.Width = this.ClientSize.Width;
            palButton.Height = buttonPanelHeight;

            // ===== 播放器區 =====
            int videoTop = infoPanel.Bottom + 20;
            int videoWidth = this.ClientSize.Width - margin * 2;
            int videoHeight = palButton.Top - videoTop - 25;

            if (videoWidth < 400)
            {
                videoWidth = 400;
            }

            if (videoHeight < 230)
            {
                videoHeight = 230;
            }

            wmpVideo.Left = (this.ClientSize.Width - videoWidth) / 2;
            wmpVideo.Top = videoTop;
            wmpVideo.Width = videoWidth;
            wmpVideo.Height = videoHeight;

            // ===== 四個按鈕自動平均排列 =====
            int buttonWidth = btnBrowser.Width;
            int buttonHeight = btnBrowser.Height;

            int sideMargin = 28;
            int availableWidth = palButton.Width - sideMargin * 2;

            int gap = (availableWidth - buttonWidth * 4) / 3;

            if (gap < 15)
            {
                gap = 15;
            }

            int totalWidth = buttonWidth * 4 + gap * 3;
            int startX = (palButton.Width - totalWidth) / 2;

            if (startX < 5)
            {
                startX = 5;
            }

            int buttonY = (palButton.Height - buttonHeight) / 2;

            btnBrowser.Location = new Point(startX, buttonY);
            btnPlay.Location = new Point(startX + (buttonWidth + gap), buttonY);
            btnPause.Location = new Point(startX + (buttonWidth + gap) * 2, buttonY);
            btnStop.Location = new Point(startX + (buttonWidth + gap) * 3, buttonY);

            // 避免被其他控制項蓋住
            palButton.BringToFront();
            btnBrowser.BringToFront();
            btnPlay.BringToFront();
            btnPause.BringToFront();
            btnStop.BringToFront();
        }

        private void frmMediaPlayer_Resize(object sender, EventArgs e)
        {
            ArrangeLayout();
        }

        // 瀏覽影片
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "選擇要播放的影片";
            ofd.Filter =
                "影片檔案|*.wmv;*.mp4;*.avi;*.mov;*.mkv|" +
                "WMV files (*.wmv)|*.wmv|" +
                "MP4 files (*.mp4)|*.mp4|" +
                "AVI files (*.avi)|*.avi|" +
                "All files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                wmpVideo.URL = ofd.FileName;
                wmpVideo.Ctlcontrols.stop();

                lblFileName.Text = "目前檔案：" + Path.GetFileName(ofd.FileName);
            }
        }

        // 播放影片
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(wmpVideo.URL))
            {
                MessageBox.Show("請先按「瀏覽」選擇影片。", "提醒",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            wmpVideo.Ctlcontrols.play();
        }

        // 暫停影片
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(wmpVideo.URL))
            {
                MessageBox.Show("目前尚未選擇影片。", "提醒",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            wmpVideo.Ctlcontrols.pause();
        }

        // 停止影片
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(wmpVideo.URL))
            {
                MessageBox.Show("目前尚未選擇影片。", "提醒",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            wmpVideo.Ctlcontrols.stop();
        }
    }
}