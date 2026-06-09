using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CastleGuardGame
{
    public class GameForm : Form
    {
        private int currentLevel;
        private int totalScore;
        private Timer gameTimer;
        private Random rng = new Random();

        private List<PictureBox> arrows    = new List<PictureBox>();
        private List<PictureBox> fireballs = new List<PictureBox>();
        private List<PictureBox> boulders  = new List<PictureBox>();
        private List<PictureBox> gargoyles = new List<PictureBox>();

        private PictureBox pbArcher;
        private PictureBox pbBackground;
        private PictureBox pbBoss;

        private ProgressBar pbCastleHP;
        private ProgressBar pbBossHP;
        private Label lblScore;
        private Label lblLevel;
        private Label lblBossHP;

        private int levelScore;
        private int castleHP;

        private bool goLeft;
        private bool goRight;
        private bool shoot;
        private bool isPaused;

        private int fireCooldown;
        private int boulderTimer;
        private int bossSpawnTimer;

        private bool bossPhase;
        private bool bossAlive;
        private int bossHP;
        private string bossDir = "Right";
        private int bossFireTimer;

        private int maxGargoyles;
        private int boulderInterval;
        private int gargoyleFireRate;

        private const int ArcherW   = 64;
        private const int ArcherH   = 64;
        private const int GargoyleW = 56;
        private const int GargoyleH = 56;
        private const int BossW     = 120;
        private const int BossH     = 120;
        private const int ArrowW    = 10;
        private const int ArrowH    = 34;
        private const int FireballW = 22;
        private const int FireballH = 22;
        private const int BoulderW  = 48;
        private const int BoulderH  = 48;

        private List<string> gargoyleDirs  = new List<string>();
        private List<int>    gargoyleTimers = new List<int>();

        public GameForm(int startLevel)
        {
            currentLevel = startLevel;
            totalScore   = 0;

            this.ClientSize      = new Size(900, 620);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.DoubleBuffered  = true;
            this.KeyPreview      = true;
            this.KeyDown        += OnKeyDown;
            this.KeyUp          += OnKeyUp;

            gameTimer          = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick    += GameLoop;

            LoadLevel(currentLevel);
        }

        private void LoadLevel(int level)
        {
            this.Text = (level == 5)
                ? "Castle Guard: Bow Master  —  BOSS LEVEL"
                : "Castle Guard: Bow Master  —  Level " + level;

            SetLevelSettings(level);
            InitGame();
            gameTimer.Start();
        }

        private void SetLevelSettings(int level)
        {
            if (level == 1)
            {
                maxGargoyles     = 2;
                boulderInterval  = 0;
                gargoyleFireRate = 90;
            }
            else if (level == 2)
            {
                maxGargoyles     = 4;
                boulderInterval  = 200;
                gargoyleFireRate = 80;
            }
            else if (level == 3)
            {
                maxGargoyles     = 6;
                boulderInterval  = 130;
                gargoyleFireRate = 70;
            }
            else if (level == 4)
            {
                maxGargoyles     = 10;
                boulderInterval  = 80;
                gargoyleFireRate = 55;
            }
            else
            {
                maxGargoyles     = 2;
                boulderInterval  = 0;
                gargoyleFireRate = 90;
            }
        }

        private void InitGame()
        {
            this.Controls.Clear();

            arrows.Clear();
            fireballs.Clear();
            boulders.Clear();
            gargoyles.Clear();
            gargoyleDirs.Clear();
            gargoyleTimers.Clear();

            levelScore    = 0;
            castleHP      = 100;
            fireCooldown  = 0;
            boulderTimer  = 0;
            bossSpawnTimer = 0;
            bossPhase     = false;
            bossAlive     = false;
            bossHP        = 15;
            bossDir       = "Right";
            bossFireTimer = 0;
            isPaused      = false;
            pbBoss        = null;

            pbBackground = MakeSprite(ResourceLoader.Background, 0, 0, 900, 620);
            this.Controls.Add(pbBackground);
            pbBackground.SendToBack();

            pbArcher = MakeSprite(ResourceLoader.Archer,
                this.ClientSize.Width / 2 - ArcherW / 2,
                this.ClientSize.Height - ArcherH - 14,
                ArcherW, ArcherH);
            this.Controls.Add(pbArcher);

            Label lblCastle     = new Label();
            lblCastle.Text      = "Castle HP";
            lblCastle.Font      = new Font("Segoe UI", 8, FontStyle.Bold);
            lblCastle.ForeColor = Color.White;
            lblCastle.BackColor = Color.FromArgb(160, 0, 0, 0);
            lblCastle.AutoSize  = true;
            lblCastle.Location  = new Point(this.ClientSize.Width - 162, 4);
            this.Controls.Add(lblCastle);

            pbCastleHP          = new ProgressBar();
            pbCastleHP.Value    = 100;
            pbCastleHP.Maximum  = 100;
            pbCastleHP.Width    = 150;
            pbCastleHP.Height   = 18;
            pbCastleHP.Location = new Point(this.ClientSize.Width - 162, 20);
            this.Controls.Add(pbCastleHP);

            lblScore           = new Label();
            lblScore.Text      = "Score: " + totalScore;
            lblScore.Font      = new Font("Segoe UI", 12, FontStyle.Bold);
            lblScore.ForeColor = Color.White;
            lblScore.BackColor = Color.FromArgb(160, 0, 0, 0);
            lblScore.AutoSize  = true;
            lblScore.Location  = new Point(8, 6);
            this.Controls.Add(lblScore);

            lblLevel           = new Label();
            lblLevel.Text      = (currentLevel == 5) ? "BOSS LEVEL" : "Level " + currentLevel + " / 5";
            lblLevel.Font      = new Font("Segoe UI", 10, FontStyle.Bold);
            lblLevel.ForeColor = (currentLevel == 5) ? Color.MediumOrchid : Color.LightYellow;
            lblLevel.BackColor = Color.FromArgb(160, 0, 0, 0);
            lblLevel.AutoSize  = true;
            lblLevel.Location  = new Point(8, 32);
            this.Controls.Add(lblLevel);

            lblBossHP           = new Label();
            lblBossHP.Text      = "BOSS HP";
            lblBossHP.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            lblBossHP.ForeColor = Color.MediumOrchid;
            lblBossHP.BackColor = Color.FromArgb(160, 0, 0, 0);
            lblBossHP.AutoSize  = true;
            lblBossHP.Location  = new Point(340, 4);
            lblBossHP.Visible   = false;
            this.Controls.Add(lblBossHP);

            pbBossHP          = new ProgressBar();
            pbBossHP.Value    = 15;
            pbBossHP.Maximum  = 15;
            pbBossHP.Width    = 220;
            pbBossHP.Height   = 22;
            pbBossHP.Location = new Point(340, 20);
            pbBossHP.Visible  = false;
            this.Controls.Add(pbBossHP);

            SpawnGargoyles(maxGargoyles);
            BringHUDToFront();
        }

        private void BringHUDToFront()
        {
            lblScore.BringToFront();
            lblLevel.BringToFront();
            pbCastleHP.BringToFront();
            lblBossHP.BringToFront();
            pbBossHP.BringToFront();
            pbArcher.BringToFront();
        }

        private void SpawnGargoyles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int spawnX = rng.Next(60, this.ClientSize.Width - GargoyleW - 60);
                int spawnY = 50 + (i % 3) * 70;
                PictureBox pb = MakeSprite(ResourceLoader.Gargoyle, spawnX, spawnY, GargoyleW, GargoyleH);
                gargoyles.Add(pb);
                gargoyleDirs.Add(rng.Next(2) == 0 ? "Right" : "Left");
                gargoyleTimers.Add(rng.Next(0, gargoyleFireRate));
                this.Controls.Add(pb);
                pb.BringToFront();
            }
        }

        private PictureBox MakeSprite(Image img, int x, int y, int w, int h)
        {
            PictureBox pb  = new PictureBox();
            pb.Image       = img;
            pb.SizeMode    = PictureBoxSizeMode.StretchImage;
            pb.BackColor   = Color.Transparent;
            pb.Left        = x;
            pb.Top         = y;
            pb.Width       = w;
            pb.Height      = h;
            return pb;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused) return;

            UpdateArcher();
            UpdateGargoyles();
            UpdateBoss();
            UpdateProjectiles();
            UpdateBoulders();
            CheckCollisions();
            CheckBossLevelTransition();
            CheckEndConditions();
        }

        private void UpdateArcher()
        {
            if (goRight && pbArcher.Right < this.ClientSize.Width - 10)
                pbArcher.Left += 10;
            if (goLeft && pbArcher.Left > 10)
                pbArcher.Left -= 10;

            if (shoot && fireCooldown <= 0 && arrows.Count < 3)
            {
                int ax = pbArcher.Left + ArcherW / 2 - ArrowW / 2;
                int ay = pbArcher.Top - ArrowH;
                PictureBox arrow = MakeSprite(ResourceLoader.Arrow, ax, ay, ArrowW, ArrowH);
                arrows.Add(arrow);
                this.Controls.Add(arrow);
                arrow.BringToFront();
                fireCooldown = 18;
                shoot = false;
            }

            if (fireCooldown > 0)
                fireCooldown--;
        }

        private void UpdateGargoyles()
        {
            for (int i = 0; i < gargoyles.Count; i++)
            {
                PictureBox g = gargoyles[i];
                int speed = (currentLevel >= 4) ? 9 : (currentLevel == 3) ? 7 : 6;

                if (gargoyleDirs[i] == "Right") g.Left += speed;
                else                            g.Left -= speed;

                if (g.Right >= this.ClientSize.Width - 10) gargoyleDirs[i] = "Left";
                if (g.Left  <= 10)                         gargoyleDirs[i] = "Right";

                gargoyleTimers[i]++;
                if (gargoyleTimers[i] >= gargoyleFireRate)
                {
                    SpawnFireball(g.Left + GargoyleW / 2 - FireballW / 2, g.Bottom);
                    gargoyleTimers[i] = 0;
                }
            }
        }

        private void UpdateBoss()
        {
            if (!bossAlive || pbBoss == null) return;

            if (bossDir == "Right") pbBoss.Left += 5;
            else                    pbBoss.Left -= 5;

            if (pbBoss.Right >= this.ClientSize.Width - 10) bossDir = "Left";
            if (pbBoss.Left  <= 10)                         bossDir = "Right";

            bossFireTimer++;
            if (bossFireTimer >= 40)
            {
                SpawnFireball(pbBoss.Left + BossW / 2 - FireballW / 2, pbBoss.Bottom);
                SpawnFireball(pbBoss.Left + 20,                         pbBoss.Bottom);
                SpawnFireball(pbBoss.Left + BossW - 20 - FireballW,    pbBoss.Bottom);
                bossFireTimer = 0;
            }

            bossSpawnTimer++;
            if (bossSpawnTimer >= 600 && gargoyles.Count < 4)
            {
                SpawnGargoyles(1);
                bossSpawnTimer = 0;
            }
        }

        private void SpawnFireball(int x, int y)
        {
            PictureBox fb = MakeSprite(ResourceLoader.Fireball, x, y, FireballW, FireballH);
            fireballs.Add(fb);
            this.Controls.Add(fb);
            fb.BringToFront();
        }

        private void UpdateProjectiles()
        {
            for (int i = 0; i < arrows.Count;    i++) arrows[i].Top    -= 18;
            for (int i = 0; i < fireballs.Count; i++) fireballs[i].Top +=  7;

            RemoveOffScreen(arrows,    a  => a.Bottom < 0);
            RemoveOffScreen(fireballs, fb => fb.Top > this.ClientSize.Height);
        }

        private void UpdateBoulders()
        {
            if (boulderInterval <= 0) return;

            for (int i = 0; i < boulders.Count; i++) boulders[i].Top += 5;

            boulderTimer++;
            if (boulderTimer >= boulderInterval)
            {
                int bx = rng.Next(20, this.ClientSize.Width - BoulderW - 20);
                PictureBox b = MakeSprite(ResourceLoader.Boulder, bx, -BoulderH, BoulderW, BoulderH);
                boulders.Add(b);
                this.Controls.Add(b);
                b.BringToFront();
                boulderTimer = 0;
            }

            RemoveOffScreen(boulders, b => b.Top > this.ClientSize.Height);
        }

        private void RemoveOffScreen(List<PictureBox> list, Func<PictureBox, bool> isDead)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (isDead(list[i]))
                {
                    this.Controls.Remove(list[i]);
                    list.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            for (int ai = arrows.Count - 1; ai >= 0; ai--)
            {
                PictureBox arrow   = arrows[ai];
                bool       hitSomething = false;

                for (int gi = gargoyles.Count - 1; gi >= 0; gi--)
                {
                    if (arrow.Bounds.IntersectsWith(gargoyles[gi].Bounds))
                    {
                        this.Controls.Remove(gargoyles[gi]);
                        gargoyles.RemoveAt(gi);
                        gargoyleDirs.RemoveAt(gi);
                        gargoyleTimers.RemoveAt(gi);
                        AddScore(50);

                        this.Controls.Remove(arrow);
                        arrows.RemoveAt(ai);
                        hitSomething = true;
                        break;
                    }
                }

                if (hitSomething) continue;

                if (bossAlive && pbBoss != null && arrow.Bounds.IntersectsWith(pbBoss.Bounds))
                {
                    bossHP--;
                    pbBossHP.Value = Math.Max(0, bossHP);
                    AddScore(10);

                    this.Controls.Remove(arrow);
                    arrows.RemoveAt(ai);
                    hitSomething = true;

                    if (bossHP <= 0)
                    {
                        this.Controls.Remove(pbBoss);
                        pbBoss    = null;
                        bossAlive = false;
                    }
                    continue;
                }

                if (hitSomething) continue;

                for (int bi = boulders.Count - 1; bi >= 0; bi--)
                {
                    if (arrow.Bounds.IntersectsWith(boulders[bi].Bounds))
                    {
                        this.Controls.Remove(boulders[bi]);
                        boulders.RemoveAt(bi);
                        AddScore(10);

                        this.Controls.Remove(arrow);
                        arrows.RemoveAt(ai);
                        break;
                    }
                }
            }

            for (int fi = fireballs.Count - 1; fi >= 0; fi--)
            {
                if (fireballs[fi].Bounds.IntersectsWith(pbArcher.Bounds))
                {
                    castleHP = Math.Max(0, castleHP - 5);
                    pbCastleHP.Value = castleHP;
                    this.Controls.Remove(fireballs[fi]);
                    fireballs.RemoveAt(fi);
                }
            }

            for (int bi = boulders.Count - 1; bi >= 0; bi--)
            {
                if (boulders[bi].Bounds.IntersectsWith(pbArcher.Bounds))
                {
                    castleHP = Math.Max(0, castleHP - 15);
                    pbCastleHP.Value = castleHP;
                    this.Controls.Remove(boulders[bi]);
                    boulders.RemoveAt(bi);
                }
            }
        }

        private void AddScore(int points)
        {
            levelScore  += points;
            totalScore  += points;
            lblScore.Text = "Score: " + totalScore;
        }

        private void CheckBossLevelTransition()
        {
            if (currentLevel != 5) return;
            if (bossPhase)         return;
            if (gargoyles.Count > 0) return;

            bossPhase = true;

            pbBoss = MakeSprite(ResourceLoader.Gargoyle,
                this.ClientSize.Width / 2 - BossW / 2, 60, BossW, BossH);
            this.Controls.Add(pbBoss);
            pbBoss.BringToFront();

            bossAlive      = true;
            bossHP         = 15;
            pbBossHP.Value = 15;
            pbBossHP.Visible  = true;
            lblBossHP.Visible = true;
            bossSpawnTimer    = 0;
        }

        private void CheckEndConditions()
        {
            if (castleHP <= 0)
            {
                gameTimer.Stop();
                OnLevelFailed();
                return;
            }

            bool normalWin  = (currentLevel != 5 && gargoyles.Count == 0);
            bool bossDefeated = (currentLevel == 5 && bossPhase && !bossAlive && gargoyles.Count == 0);

            if (normalWin || bossDefeated)
            {
                gameTimer.Stop();
                OnLevelComplete();
            }
        }

        private void OnLevelComplete()
        {
            if (currentLevel < 5)
            {
                string msg = "Level " + currentLevel + " complete!\n"
                           + "Level score: " + levelScore + "\n"
                           + "Total score: " + totalScore + "\n\n"
                           + "Proceed to Level " + (currentLevel + 1) + "?";

                DialogResult dr = MessageBox.Show(msg, "Level Complete!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    currentLevel++;
                    castleHP = 100;
                    LoadLevel(currentLevel);
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                string msg = "You defeated the Boss!\n\n"
                           + "FINAL SCORE: " + totalScore + "\n\n"
                           + "Congratulations — you completed all 5 levels!\n\n"
                           + "Play again from Level 1?";

                DialogResult dr = MessageBox.Show(msg, "You Win!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    currentLevel = 1;
                    totalScore   = 0;
                    LoadLevel(1);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void OnLevelFailed()
        {
            string msg = "Castle HP reached zero!\n\n"
                       + "Score this level: " + levelScore + "\n"
                       + "Total score: " + totalScore + "\n\n"
                       + "Retry Level " + currentLevel + "?";

            DialogResult dr = MessageBox.Show(msg, "Game Over",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                totalScore -= levelScore;
                if (totalScore < 0) totalScore = 0;
                LoadLevel(currentLevel);
            }
            else
            {
                this.Close();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left  || e.KeyCode == Keys.A) goLeft  = true;
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) goRight = true;
            if (e.KeyCode == Keys.Space) shoot = true;

            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.P)
            {
                gameTimer.Stop();
                isPaused = true;

                DialogResult dr = MessageBox.Show(
                    "Game Paused\n\nReturn to Main Menu?",
                    "Paused",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                    this.Close();
                else
                {
                    isPaused = false;
                    gameTimer.Start();
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left  || e.KeyCode == Keys.A) goLeft  = false;
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) goRight = false;
            if (e.KeyCode == Keys.Space) shoot = false;
        }
    }
}
