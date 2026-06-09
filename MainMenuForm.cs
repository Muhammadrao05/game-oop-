using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CastleGuardGame
{
    class BackgroundPanel : Panel
    {
        public BackgroundPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
    }

    public class MainMenuForm : Form
    {
        private Timer animTimer;
        private float pulse = 0f;

        private BackgroundPanel bgPanel;

        private static readonly Random starRng = new Random(42);
        private static readonly int[]   starX  = new int[80];
        private static readonly int[]   starY  = new int[80];
        private static readonly int[]   starSz = new int[80];

        static MainMenuForm()
        {
            for (int i = 0; i < 80; i++)
            {
                starX[i]  = starRng.Next(0, 800);
                starY[i]  = starRng.Next(0, 600);
                starSz[i] = starRng.Next(0, 5) == 0 ? 2 : 1;
            }
        }

        public MainMenuForm()
        {
            this.Text            = "Castle Guard: Bow Master";
            this.ClientSize      = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.DoubleBuffered  = true;
            this.BackColor       = Color.FromArgb(10, 5, 30);

            bgPanel          = new BackgroundPanel();
            bgPanel.Size     = new Size(800, 600);
            bgPanel.Location = new Point(0, 0);
            bgPanel.Paint   += OnBgPaint;
            this.Controls.Add(bgPanel);

            BuildUI();

            animTimer          = new Timer();
            animTimer.Interval = 30;
            animTimer.Tick    += OnAnimTick;
            animTimer.Start();
        }

        private void BuildUI()
        {
            Panel cardPanel = new Panel();
            cardPanel.Size      = new Size(300, 360);
            cardPanel.Location  = new Point((800 - 300) / 2, 170);
            cardPanel.BackColor = Color.FromArgb(180, 14, 10, 40);
            bgPanel.Controls.Add(cardPanel);

            Label title    = new Label();
            title.Text     = "Castle Guard";
            title.Font     = new Font("Impact", 38, FontStyle.Bold);
            title.ForeColor = Color.Gold;
            title.BackColor = Color.Transparent;
            title.AutoSize  = false;
            title.Size      = new Size(800, 58);
            title.Location  = new Point(0, 72);
            title.TextAlign = ContentAlignment.MiddleCenter;
            bgPanel.Controls.Add(title);

            Label subtitle    = new Label();
            subtitle.Text     = "Bow Master";
            subtitle.Font     = new Font("Segoe UI", 16, FontStyle.Italic);
            subtitle.ForeColor = Color.LightCyan;
            subtitle.BackColor = Color.Transparent;
            subtitle.AutoSize  = false;
            subtitle.Size      = new Size(800, 32);
            subtitle.Location  = new Point(0, 130);
            subtitle.TextAlign = ContentAlignment.MiddleCenter;
            bgPanel.Controls.Add(subtitle);

            int btnX   = 25;
            int btnY   = 28;
            int btnGap = 74;

            Button btnNew    = MakeButton("  \u25B6  Start New Game", btnX, btnY, Color.FromArgb(20, 110, 60));
            btnNew.Click    += (s, e) => StartLevel(1);
            cardPanel.Controls.Add(btnNew);

            Button btnSelect  = MakeButton("  \u2261  Select Level", btnX, btnY + btnGap, Color.FromArgb(30, 80, 160));
            btnSelect.Click  += (s, e) => OpenLevelSelect();
            cardPanel.Controls.Add(btnSelect);

            Button btnHow     = MakeButton("  ?  How to Play", btnX, btnY + btnGap * 2, Color.FromArgb(100, 80, 20));
            btnHow.Click     += (s, e) => new HowToPlayForm().ShowDialog(this);
            cardPanel.Controls.Add(btnHow);

            Button btnExit    = MakeButton("  \u2715  Exit", btnX, btnY + btnGap * 3, Color.FromArgb(140, 30, 30));
            btnExit.Click    += (s, e) => Application.Exit();
            cardPanel.Controls.Add(btnExit);

            Label version    = new Label();
            version.Text     = "v2.0";
            version.Font     = new Font("Segoe UI", 8);
            version.ForeColor = Color.FromArgb(80, 80, 80);
            version.BackColor = Color.Transparent;
            version.AutoSize  = true;
            version.Location  = new Point(762, 582);
            bgPanel.Controls.Add(version);
        }

        private Button MakeButton(string text, int x, int y, Color backColor)
        {
            Button btn                         = new Button();
            btn.Text                           = text;
            btn.Font                           = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.ForeColor                      = Color.White;
            btn.BackColor                      = backColor;
            btn.FlatStyle                      = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor     = Color.FromArgb(120, 180, 255);
            btn.FlatAppearance.BorderSize      = 1;
            btn.Size                           = new Size(250, 54);
            btn.Location                       = new Point(x, y);
            btn.Cursor                         = Cursors.Hand;
            btn.TextAlign                      = ContentAlignment.MiddleLeft;
            btn.Padding                        = new Padding(14, 0, 0, 0);
            return btn;
        }

        private void StartLevel(int level)
        {
            animTimer.Stop();
            this.Hide();
            GameForm game      = new GameForm(level);
            game.FormClosed   += (s, e) => { this.Show(); animTimer.Start(); };
            game.Show();
        }

        private void OpenLevelSelect()
        {
            LevelSelectForm ls  = new LevelSelectForm();
            ls.LevelChosen     += (lvl) => StartLevel(lvl);
            ls.ShowDialog(this);
        }

        private void OnAnimTick(object sender, EventArgs e)
        {
            pulse += 0.05f;
            if (pulse > (float)(Math.PI * 2))
                pulse -= (float)(Math.PI * 2);

            bgPanel.Invalidate();
        }

        private void OnBgPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (LinearGradientBrush bg = new LinearGradientBrush(
                bgPanel.ClientRectangle,
                Color.FromArgb(10, 5, 30),
                Color.FromArgb(28, 10, 58),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(bg, bgPanel.ClientRectangle);
            }

            for (int i = 0; i < 80; i++)
            {
                float twinkle = (float)Math.Sin(pulse * 1.4f + i * 0.4f) * 0.4f + 0.6f;
                int   alpha   = (int)(twinkle * 210);
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(alpha, 210, 215, 255)))
                    g.FillEllipse(sb, starX[i], starY[i], starSz[i], starSz[i]);
            }

            float glow     = (float)Math.Sin(pulse) * 0.35f + 0.65f;
            int   glowA    = (int)(glow * 160);
            using (Pen glowPen = new Pen(Color.FromArgb(glowA, 80, 140, 255), 2))
                g.DrawRectangle(glowPen, 30, 30, 740, 540);

            using (Pen linePen = new Pen(Color.FromArgb(130, 200, 160, 50), 1))
            {
                g.DrawLine(linePen, 180, 165, 620, 165);
                g.DrawLine(linePen, 180, 168, 620, 168);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            animTimer.Stop();
            base.OnFormClosed(e);
        }
    }
}
