using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CastleGuardGame
{
    public class LevelSelectForm : Form
    {
        public event Action<int> LevelChosen;

        public LevelSelectForm()
        {
            this.Text = "Select Level";
            this.ClientSize = new Size(560, 520);
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
            title.Text = "Select Level";
            title.Font = new Font("Impact", 26, FontStyle.Bold);
            title.ForeColor = Color.Gold;
            title.BackColor = Color.Transparent;
            title.AutoSize = false;
            title.Size = new Size(560, 50);
            title.Location = new Point(0, 18);
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);

            string[] names = {
                "Level 1  —  Scout Patrol",
                "Level 2  —  Raiding Party",
                "Level 3  —  Siege Attack",
                "Level 4  —  Overwhelming Horde",
                "Level 5  —  Boss Level"
            };

            string[] descs = {
                "2 gargoyles  |  No boulders  |  Easy",
                "4 gargoyles  |  Few boulders  |  Medium",
                "6 gargoyles  |  More boulders  |  Hard",
                "10 gargoyles  |  Heavy boulders  |  Very Hard",
                "Gargoyles + Giant Boss  |  Extreme"
            };

            Color[] colors = {
                Color.FromArgb(20, 120, 60),
                Color.FromArgb(30, 100, 160),
                Color.FromArgb(140, 100, 20),
                Color.FromArgb(160, 50, 20),
                Color.FromArgb(130, 20, 130)
            };

            int startY = 88;
            for (int i = 0; i < 5; i++)
            {
                int levelNum = i + 1;
                Panel card = MakeLevelCard(names[i], descs[i], colors[i], levelNum);
                card.Location = new Point(30, startY + i * 80);
                this.Controls.Add(card);
            }

            Button btnBack = new Button();
            btnBack.Text = "Back";
            btnBack.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.BackColor = Color.FromArgb(60, 60, 60);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderColor = Color.Gray;
            btnBack.Size = new Size(120, 36);
            btnBack.Location = new Point(220, 480);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnBack);
        }

        private Panel MakeLevelCard(string name, string desc, Color baseColor, int level)
        {
            Panel card = new Panel();
            card.Size = new Size(500, 68);
            card.BackColor = Color.FromArgb(25, 20, 50);
            card.Cursor = Cursors.Hand;

            Label lblName = new Label();
            lblName.Text = name;
            lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblName.ForeColor = Color.White;
            lblName.BackColor = Color.Transparent;
            lblName.AutoSize = false;
            lblName.Size = new Size(380, 28);
            lblName.Location = new Point(70, 8);
            card.Controls.Add(lblName);

            Label lblDesc = new Label();
            lblDesc.Text = desc;
            lblDesc.Font = new Font("Segoe UI", 9);
            lblDesc.ForeColor = Color.LightGray;
            lblDesc.BackColor = Color.Transparent;
            lblDesc.AutoSize = false;
            lblDesc.Size = new Size(380, 22);
            lblDesc.Location = new Point(70, 36);
            card.Controls.Add(lblDesc);

            Panel colorBar = new Panel();
            colorBar.BackColor = baseColor;
            colorBar.Size = new Size(10, 68);
            colorBar.Location = new Point(0, 0);
            card.Controls.Add(colorBar);

            Label lblNum = new Label();
            lblNum.Text = level == 5 ? "B" : level.ToString();
            lblNum.Font = new Font("Impact", 22, FontStyle.Bold);
            lblNum.ForeColor = baseColor;
            lblNum.BackColor = Color.Transparent;
            lblNum.AutoSize = false;
            lblNum.Size = new Size(50, 68);
            lblNum.Location = new Point(14, 0);
            lblNum.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(lblNum);

            EventHandler onClick = (s, e) =>
            {
                if (LevelChosen != null)
                    LevelChosen(level);
                this.Close();
            };

            card.Click += onClick;
            lblName.Click += onClick;
            lblDesc.Click += onClick;
            colorBar.Click += onClick;
            lblNum.Click += onClick;

            return card;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Color.FromArgb(80, 100, 140, 255), 2))
                e.Graphics.DrawRectangle(pen, 1, 1, this.ClientSize.Width - 3, this.ClientSize.Height - 3);
            using (Pen line = new Pen(Color.FromArgb(100, 200, 160, 60), 1))
                e.Graphics.DrawLine(line, 30, 78, 530, 78);
        }
    }
}
