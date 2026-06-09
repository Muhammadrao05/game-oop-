using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CastleGuardGame
{
    public class HowToPlayForm : Form
    {
        public HowToPlayForm()
        {
            this.Text = "How to Play";
            this.ClientSize = new Size(580, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(12, 8, 35);

            BuildUI();
        }

        private void BuildUI()
        {
            Label title = new Label();
            title.Text = "How to Play";
            title.Font = new Font("Impact", 26, FontStyle.Bold);
            title.ForeColor = Color.Gold;
            title.BackColor = Color.Transparent;
            title.AutoSize = false;
            title.Size = new Size(580, 50);
            title.Location = new Point(0, 14);
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);

            Panel panel = new Panel();
            panel.BackColor = Color.FromArgb(20, 18, 45);
            panel.Size = new Size(520, 440);
            panel.Location = new Point(30, 72);
            this.Controls.Add(panel);

            int y = 18;
            int gap = 28;

            AddSection(panel, "CONTROLS", Color.Cyan, ref y);
            AddRow(panel, "←  /  →  Arrow Keys", "Move the archer left and right", ref y, gap);
            AddRow(panel, "SPACE", "Fire an arrow  (max 3 in the air at once)", ref y, gap);
            AddRow(panel, "ESC", "Pause / return to Main Menu", ref y, gap);
            y += 10;

            AddSection(panel, "OBJECTIVE", Color.LightGreen, ref y);
            AddRow(panel, "Shoot gargoyles", "Each hit kills a gargoyle (+50 pts)", ref y, gap);
            AddRow(panel, "Avoid fireballs", "Each fireball hit  -5 Castle HP", ref y, gap);
            AddRow(panel, "Shoot boulders", "Destroy boulders to stop damage  (+10 pts)", ref y, gap);
            AddRow(panel, "Survive!", "Castle HP = 0 means Game Over", ref y, gap);
            y += 10;

            AddSection(panel, "LEVELS", Color.Orange, ref y);
            AddRow(panel, "Level 1", "2 gargoyles, no boulders", ref y, gap);
            AddRow(panel, "Level 2", "4 gargoyles, a few boulders", ref y, gap);
            AddRow(panel, "Level 3", "6 gargoyles, more boulders", ref y, gap);
            AddRow(panel, "Level 4", "10 gargoyles, heavy boulders", ref y, gap);
            AddRow(panel, "Level 5", "Gargoyles, then a Giant Boss (15 HP)", ref y, gap);

            Button btnClose = new Button();
            btnClose.Text = "Got it!";
            btnClose.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.FromArgb(20, 120, 60);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderColor = Color.LightGreen;
            btnClose.Size = new Size(140, 40);
            btnClose.Location = new Point(220, 520);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnClose);
        }

        private void AddSection(Panel panel, string text, Color color, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = color;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize = false;
            lbl.Size = new Size(500, 24);
            lbl.Location = new Point(12, y);
            panel.Controls.Add(lbl);
            y += 26;

            Panel line = new Panel();
            line.BackColor = Color.FromArgb(60, color);
            line.Size = new Size(496, 1);
            line.Location = new Point(12, y);
            panel.Controls.Add(line);
            y += 8;
        }

        private void AddRow(Panel panel, string key, string desc, ref int y, int gap)
        {
            Label lblKey = new Label();
            lblKey.Text = key;
            lblKey.Font = new Font("Courier New", 9, FontStyle.Bold);
            lblKey.ForeColor = Color.LightYellow;
            lblKey.BackColor = Color.FromArgb(40, 255, 255, 100);
            lblKey.AutoSize = false;
            lblKey.Size = new Size(150, 20);
            lblKey.Location = new Point(12, y);
            lblKey.TextAlign = ContentAlignment.MiddleLeft;
            lblKey.Padding = new Padding(4, 0, 0, 0);
            panel.Controls.Add(lblKey);

            Label lblDesc = new Label();
            lblDesc.Text = desc;
            lblDesc.Font = new Font("Segoe UI", 9);
            lblDesc.ForeColor = Color.LightGray;
            lblDesc.BackColor = Color.Transparent;
            lblDesc.AutoSize = false;
            lblDesc.Size = new Size(340, 20);
            lblDesc.Location = new Point(168, y);
            panel.Controls.Add(lblDesc);

            y += gap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Color.FromArgb(80, 100, 140, 255), 2))
                e.Graphics.DrawRectangle(pen, 1, 1, this.ClientSize.Width - 3, this.ClientSize.Height - 3);
        }
    }
}
