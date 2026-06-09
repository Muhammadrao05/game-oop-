using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CastleGuardGame
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;

        private List<PictureBox> arrows    = new List<PictureBox>();
        private List<PictureBox> fireballs = new List<PictureBox>();
        private List<PictureBox> boulders  = new List<PictureBox>();

        private PictureBox pbArcher, pbGargoyle1, pbGargoyle2, pbBackground;
        private ProgressBar pbHealth;
        private Label lblScore;

        private readonly Random rng = new Random();

        private int score;
        private bool gargoyle1Alive, gargoyle2Alive;
        private int g1Timer, g2Timer, boulderTimer;
        private string g1Dir = "Right", g2Dir = "Left";
        private bool goLeft, goRight, shoot;

        private const int FireCooldown    = 80;
        private const int BoulderInterval = 120;

        private const int ArcherW   = 48;
        private const int ArcherH   = 48;
        private const int GargoyleW = 52;
        private const int GargoyleH = 52;
        private const int ArrowW    = 10;
        private const int ArrowH    = 30;
        private const int FireballW = 20;
        private const int FireballH = 20;
        private const int BoulderW  = 36;
        private const int BoulderH  = 36;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            KeyPreview = true;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            gameTimer = new Timer { Interval = 16 };
            gameTimer.Tick += GameLoop;

            InitGame();
            ShowInstructions();
        }

        private void InitGame()
        {
            arrows.Clear();
            fireballs.Clear();
            boulders.Clear();

            score = 0;
            gargoyle1Alive = gargoyle2Alive = true;
            g1Timer = g2Timer = boulderTimer = 0;

            pbBackground = Sprite(ResourceLoader.Background, 0, 0, ClientSize.Width, ClientSize.Height);
            Controls.Add(pbBackground);
            pbBackground.SendToBack();

            pbArcher = Sprite(ResourceLoader.Archer, ClientSize.Width / 2 - ArcherW / 2, ClientSize.Height - ArcherH - 10, ArcherW, ArcherH);
            Controls.Add(pbArcher);

            pbHealth = new ProgressBar
            {
                Value    = 100,
                Maximum  = 100,
                Width    = 130,
                Height   = 18,
                Location = new Point(ClientSize.Width - 145, 18)
            };
            Controls.Add(pbHealth);

            pbGargoyle1 = Sprite(ResourceLoader.Gargoyle, rng.Next(50, ClientSize.Width - GargoyleW - 50), 60, GargoyleW, GargoyleH);
            pbGargoyle2 = Sprite(ResourceLoader.Gargoyle, rng.Next(50, ClientSize.Width - GargoyleW - 50), 140, GargoyleW, GargoyleH);
            Controls.Add(pbGargoyle1);
            Controls.Add(pbGargoyle2);

            lblScore = new Label
            {
                Text      = "Score: 0",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                AutoSize  = true,
                Location  = new Point(8, 8)
            };
            Controls.Add(lblScore);

            var lblHP = new Label
            {
                Text      = "Castle HP",
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                AutoSize  = true,
                Location  = new Point(ClientSize.Width - 145, 2)
            };
            Controls.Add(lblHP);

            lblScore.BringToFront();
            lblHP.BringToFront();
            pbHealth.BringToFront();
            pbArcher.BringToFront();
            pbGargoyle1.BringToFront();
            pbGargoyle2.BringToFront();
        }

        private PictureBox Sprite(Image img, int x, int y, int w, int h)
        {
            return new PictureBox
            {
                Image     = img,
                SizeMode  = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Left = x, Top = y, Width = w, Height = h
            };
        }

        private void ShowInstructions()
        {
            var panel = new Panel
            {
                Size        = new Size(370, 310),
                BackColor   = Color.FromArgb(220, 15, 15, 35),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Location = new Point((ClientSize.Width - panel.Width) / 2, (ClientSize.Height - panel.Height) / 2);

            panel.Controls.Add(new Label
            {
                Text      = "⚔   Castle Guard: Bow Master   ⚔",
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize  = false,
                Size      = new Size(360, 34),
                Location  = new Point(5, 12),
                TextAlign = ContentAlignment.MiddleCenter
            });

            panel.Controls.Add(new Label
            {
                Text =
                    "CONTROLS\r\n" +
                    "   ← / →       Move archer left / right\r\n" +
                    "   SPACE        Fire arrow (max 3 in air)\r\n\r\n" +
                    "GOAL\r\n" +
                    "   Shoot both gargoyles to win!\r\n" +
                    "   Don't let fireballs or boulders\r\n" +
                    "   drain castle health to zero.\r\n\r\n" +
                    "SCORING\r\n" +
                    "   Gargoyle killed      +50 pts\r\n" +
                    "   Boulder destroyed  +10 pts",
                Font      = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize  = false,
                Size      = new Size(360, 210),
                Location  = new Point(5, 52)
            });

            var btn = new Button
            {
                Text      = "▶   Start Game",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 140, 60),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(160, 36),
                Location  = new Point(105, 264)
            };
            btn.Click += (s, e) => { Controls.Remove(panel); gameTimer.Start(); Focus(); };
            panel.Controls.Add(btn);

            Controls.Add(panel);
            panel.BringToFront();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            UpdatePlayer();
            UpdateEnemies();
            UpdateProjectiles();
            CheckCollisions();
            CheckEndConditions();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)  goLeft  = true;
            if (e.KeyCode == Keys.Right) goRight = true;
            if (e.KeyCode == Keys.Space) shoot   = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)  goLeft  = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Space) shoot   = false;
        }

        private void UpdatePlayer()
        {
            if (goRight && pbArcher.Right  < ClientSize.Width - 10) pbArcher.Left += 12;
            if (goLeft  && pbArcher.Left   > 10)                     pbArcher.Left -= 12;

            if (shoot && arrows.Count < 3)
            {
                var a = Sprite(ResourceLoader.Arrow, pbArcher.Left + ArcherW / 2 - ArrowW / 2, pbArcher.Top - ArrowH, ArrowW, ArrowH);
                arrows.Add(a);
                Controls.Add(a);
                a.BringToFront();
                shoot = false;
            }
        }

        private void SpawnFireball(PictureBox src)
        {
            var fb = Sprite(ResourceLoader.Fireball, src.Left + GargoyleW / 2 - FireballW / 2, src.Bottom, FireballW, FireballH);
            fireballs.Add(fb);
            Controls.Add(fb);
            fb.BringToFront();
        }

        private void UpdateEnemies()
        {
            if (gargoyle1Alive)
            {
                pbGargoyle1.Left += g1Dir == "Right" ? 6 : -6;
                if (pbGargoyle1.Right >= ClientSize.Width - 10) g1Dir = "Left";
                if (pbGargoyle1.Left  <= 10)                    g1Dir = "Right";
                if (++g1Timer > FireCooldown) { SpawnFireball(pbGargoyle1); g1Timer = 0; }
            }

            if (gargoyle2Alive)
            {
                pbGargoyle2.Left += g2Dir == "Right" ? 8 : -8;
                if (pbGargoyle2.Right >= ClientSize.Width - 10) g2Dir = "Left";
                if (pbGargoyle2.Left  <= 10)                    g2Dir = "Right";
                if (++g2Timer > FireCooldown + 10) { SpawnFireball(pbGargoyle2); g2Timer = 0; }
            }
        }

        private void UpdateProjectiles()
        {
            foreach (var a  in arrows)    a.Top  -= 16;
            foreach (var fb in fireballs) fb.Top += 8;
            foreach (var b  in boulders)  b.Top  += 6;

            if (++boulderTimer > BoulderInterval)
            {
                var b = Sprite(ResourceLoader.Boulder, rng.Next(20, ClientSize.Width - BoulderW - 20), -BoulderH, BoulderW, BoulderH);
                boulders.Add(b);
                Controls.Add(b);
                b.BringToFront();
                boulderTimer = 0;
            }

            Cleanup(arrows,    a  => a.Bottom  < 0);
            Cleanup(fireballs, fb => fb.Top    > ClientSize.Height);
            Cleanup(boulders,  b  => b.Top     > ClientSize.Height);
        }

        private void Cleanup(List<PictureBox> list, Func<PictureBox, bool> dead)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (dead(list[i])) { Controls.Remove(list[i]); list.RemoveAt(i); }
            }
        }

        private void CheckCollisions()
        {
            foreach (var a in arrows)
            {
                if (gargoyle1Alive && a.Bounds.IntersectsWith(pbGargoyle1.Bounds))
                    { pbGargoyle1.Hide(); gargoyle1Alive = false; score += 50; }
                if (gargoyle2Alive && a.Bounds.IntersectsWith(pbGargoyle2.Bounds))
                    { pbGargoyle2.Hide(); gargoyle2Alive = false; score += 50; }
                foreach (var b in boulders)
                    if (a.Bounds.IntersectsWith(b.Bounds)) { b.Top = ClientSize.Height + 500; score += 10; }
            }

            foreach (var fb in fireballs)
                if (fb.Bounds.IntersectsWith(pbArcher.Bounds))
                    { pbHealth.Value = Math.Max(0, pbHealth.Value - 5); fb.Top = ClientSize.Height + 500; }

            foreach (var b in boulders)
                if (b.Bounds.IntersectsWith(pbArcher.Bounds))
                    { pbHealth.Value = Math.Max(0, pbHealth.Value - 15); b.Top = ClientSize.Height + 500; }

            lblScore.Text = "Score: " + score;
        }

        private void CheckEndConditions()
        {
            if (pbHealth.Value <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over! Final score: " + score);
                Restart();
            }
            else if (!gargoyle1Alive && !gargoyle2Alive)
            {
                gameTimer.Stop();
                MessageBox.Show("You Win! Final score: " + score);
                Restart();
            }
        }

        private void Restart()
        {
            Controls.Clear();
            InitGame();
            ShowInstructions();
        }
    }
}
