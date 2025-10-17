using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Media;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacShehab.Properties;
using System.IO;
using NAudio.Wave;


namespace TicTacShehab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Width = 1400;
            this.Height = 850;

            this.MinimumSize = new Size(1300, 800);
            this.MaximumSize = new Size(1400, 850);

            PlayMusicLoop("ClassicMusic.mp3");
        }

        public enum enGameTheme { Classic, Neon, Cute }
        public enum enGameMode { HumanVsComputer , HumanVsHuman }
        public enum enBoardSize { ThreeByThree, FourByFour}

        public struct stGameOptions
        {
            public enGameTheme Theme;
            public enGameMode Mode;
            public enBoardSize BoardSize;
        }

        public stGameOptions GameSettings;

        private void LoadOptionsUI()
        {
            // Theme
            rbClassic.Checked = (GameSettings.Theme == enGameTheme.Classic);
            rbNeon.Checked = (GameSettings.Theme == enGameTheme.Neon);
            rbCute.Checked = (GameSettings.Theme == enGameTheme.Cute);

            // Mode
            rbHuman.Checked = (GameSettings.Mode == enGameMode.HumanVsHuman);
            rbComputer.Checked = (GameSettings.Mode == enGameMode.HumanVsComputer);

            // Board Size
            rb3x3.Checked = (GameSettings.BoardSize == enBoardSize.ThreeByThree);
            rb4x4.Checked = (GameSettings.BoardSize == enBoardSize.FourByFour);
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            PanelMainMenu.Visible = false;
            PanelOptions.Visible = true;

            LoadOptionsUI();
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            GameSettings.Theme = enGameTheme.Classic;
            GameSettings.Mode = enGameMode.HumanVsComputer;
            GameSettings.BoardSize = enBoardSize.ThreeByThree;

            LoadOptionsUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Theme
            if (rbClassic.Checked) GameSettings.Theme = enGameTheme.Classic;
            else if (rbNeon.Checked) GameSettings.Theme = enGameTheme.Neon;
            else if (rbCute.Checked) GameSettings.Theme = enGameTheme.Cute;

            // Mode
            if (rbHuman.Checked) GameSettings.Mode = enGameMode.HumanVsHuman;
            else if (rbComputer.Checked) GameSettings.Mode = enGameMode.HumanVsComputer;

            // Board Size
            if (rb3x3.Checked) GameSettings.BoardSize = enBoardSize.ThreeByThree;
            else if (rb4x4.Checked) GameSettings.BoardSize = enBoardSize.FourByFour;

            PanelMainMenu.Visible = true;
            PanelOptions.Visible = false;
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            PanelMainMenu.Visible = false;
            PanelOptions.Visible = false;
            PanelGame.Visible = true;
            tbPlayer1.Focus();
            
            InitializeGame();
        }

        private void InitializeGame()
        {
            ApplyTheme(GameSettings.Theme);

            PlayerTurn = enPlayer.Player1;
            lblTurn.Text = tbPlayer1.Text;

            PanelGame.Invalidate();
            InitializeBoardButtonsForComputer();

        }

        private void InitializeBoardButtonsForComputer()
        {

            if (GameSettings.BoardSize == enBoardSize.FourByFour)
            {
                gameButtons = new List<Button>
                {
                    button1, button2, button3, button4, button5,
                    button6, button7, button8, button9,
                    button12, button13, button14, button15,
                    button16, button17, button18
                };
            }
            else
            {
                gameButtons = new List<Button>
                {
                    button1, button2, button3, button4, button5,
                    button6, button7, button8, button9
                };
            }
            // Reset tags
            foreach (var btn in gameButtons)
            {
                btn.Tag = "?";
                btn.Text = "";
            }
        }

        private void ApplyTheme(enGameTheme theme)
        {
            switch (theme)
            {
                case enGameTheme.Classic:
                    PanelGame.BackgroundImage = Resources.XOClassic2;
                    pictureBox1.BackgroundImage = Resources.gbClassic1;
                    break;
                case enGameTheme.Neon:
                    PanelGame.BackgroundImage = Resources.backNeon;
                    pictureBox1.BackgroundImage = Resources.gbNeon11;
                    break;
                case enGameTheme.Cute:
                    PanelGame.BackgroundImage = Resources.backCute1;
                    pictureBox1.BackgroundImage = Resources.gbCute1;
                    break;
            }
        }
        
        private void SetupBoard(enBoardSize size, Graphics g)
        {

            using ( g = PanelGame.CreateGraphics())
            {
                Pen pen = new Pen(Color.FromArgb(255, 255, 0, 0), 15);
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                
                if (size == enBoardSize.ThreeByThree)
                {
                    g.DrawLine(pen, 600, 260, 600, 700);
                    g.DrawLine(pen, 765, 260, 765, 700);

                    g.DrawLine(pen, 430, 400, 930, 400);
                    g.DrawLine(pen, 430, 550, 930, 550);

                    button14.Visible = false;
                    button13.Visible = false;
                    button12.Visible = false;
                    button15.Visible = false;
                    button18.Visible = false;
                    button17.Visible = false;
                    button16.Visible = false;
                }
                else if (size == enBoardSize.FourByFour)
                {
                    g.DrawLine(pen, 600, 120, 600, 700);
                    g.DrawLine(pen, 765, 120, 765, 700);
                    g.DrawLine(pen, 930, 120, 930, 700);

                    g.DrawLine(pen, 450, 250, 1050, 250);
                    g.DrawLine(pen, 450, 410, 1050, 410);
                    g.DrawLine(pen, 450, 560, 1050, 560);

                    button14.Visible = true;
                    button13.Visible = true;
                    button12.Visible = true;
                    button15.Visible = true;
                    button18.Visible = true;
                    button17.Visible = true;
                    button16.Visible = true;
                }
            }
        }

        private void PanelGame_Paint(object sender, PaintEventArgs e)
        {
            SetupBoard(GameSettings.BoardSize, e.Graphics);
        }


        stGameStatus GameStatus;
        enPlayer PlayerTurn;

        enum enPlayer
        {
            Player1,
            Player2
        }

        enum enWinner
        {
            Player1,
            Player2,
            Draw,
            GameInProgress
        }

        struct stGameStatus
        {
            public enWinner Winner;
            public bool GameOver;
            public short PlayCount;
        }

        void EndGame()
        {
            lblTurn.Text = "GameOver";

            switch (GameStatus.Winner)
            {
                case enWinner.Player1:
                    lblWinner.Text = tbPlayer1.Text;

                    rtbGames.Text += System.Environment.NewLine + "Winner of Game "
                        + lblRound.Tag.ToString() + " 🏅 : " + tbPlayer1.Text;
                    rtbGames.SelectionStart = rtbGames.Text.Length;
                    rtbGames.ScrollToCaret();

                    lblSocrePlayer1.Tag = Convert.ToByte(lblSocrePlayer1.Tag) + 1;
                    lblSocrePlayer1.Text = ":    " + lblSocrePlayer1.Tag.ToString();

                    PlayEffect("WinSound.wav");

                    break;
                case enWinner.Player2:
                    lblWinner.Text = tbPlayer2.Text;

                    rtbGames.Text += System.Environment.NewLine + "Winner of Game "
                        + lblRound.Tag.ToString() + " 🏅 : " + tbPlayer2.Text;
                    rtbGames.SelectionStart = rtbGames.Text.Length;
                    rtbGames.ScrollToCaret();

                    lblSocrePlayer2.Tag = Convert.ToByte(lblSocrePlayer2.Tag) + 1;
                    lblSocrePlayer2.Text = ":    " + lblSocrePlayer2.Tag.ToString();

                    PlayEffect("WinSound.wav");

                    break;
                default:
                    lblWinner.Text = "    Draw";
                    break;
            }

        }

        public bool CheckValues(params Button[] buttons)
        {
            if (buttons.Length < 3)
                return false;

            string firstTag = buttons[0].Tag.ToString();

            if (firstTag == "?")
                return false;

            foreach (var btn in buttons)
            {
                if (btn.Tag.ToString() != firstTag)
                    return false;
            }

            foreach (var btn in buttons)
            {
                btn.BackColor = Color.GreenYellow;
            }

            if (firstTag == "X")
            {
                GameStatus.Winner = enWinner.Player1;
            }
            else
            {
                GameStatus.Winner = enWinner.Player2;
            }

            GameStatus.GameOver = true;
            EndGame();

            btnNextRound.Enabled = true;
            btnNextRound.BackColor = Color.Lime;

            return true;
        }

        public void CheckWinner()
        {
            if (GameSettings.BoardSize == enBoardSize.ThreeByThree)
            {
            //checked rows
            //check Row1
            if (CheckValues(button1, button2, button3))
                return;

            //check Row2
            if (CheckValues(button4, button5, button6))
                return;

            //check Row3
            if (CheckValues(button7, button8, button9))
                return;

            //checked cols
            //check Col1
            if (CheckValues(button1, button4, button7))
                return;

            //check Col2
            if (CheckValues(button2, button5, button8))
                return;

            //check Col3
            if (CheckValues(button3, button6, button9))
                return;

            //checked diagonals
            //check Diagonal1
            if (CheckValues(button1, button5, button9))
                return;

            //check Diagonal2
            if (CheckValues(button3, button5, button7))
                return;
            }
            else
            {
                if (CheckValues(button12, button13, button14, button15)) return;
                if (CheckValues(button1, button2, button3, button16)) return;
                if (CheckValues(button4, button5, button6, button17)) return;
                if (CheckValues(button7, button8, button9, button18)) return;

                if (CheckValues(button12, button1, button4, button7)) return;
                if (CheckValues(button13, button2, button5, button8)) return;
                if (CheckValues(button14, button3, button6, button9)) return;
                if (CheckValues(button14, button3, button6, button9)) return;
                if (CheckValues(button15, button16, button17, button18)) return;

                if (CheckValues(button12, button2, button6, button18)) return;
                if (CheckValues(button15, button3, button5, button7)) return;
            }


        }

        public void Buttonimage(Button btn)
        {
            if (PlayerTurn == enPlayer.Player1)
            {
                if (GameSettings.Theme == enGameTheme.Classic)
                {
                    btn.Image = Resources.X;
                }
                else if (GameSettings.Theme == enGameTheme.Neon)
                {
                    //btn.Image = Resources.XNeon3;
                    btn.BackgroundImage = Resources.XNeon4;
                }
                else
                {
                    btn.BackgroundImage = Resources.XCute;
                }
            }
            else
            {
                {
                    if (GameSettings.Theme == enGameTheme.Classic)
                    {
                        btn.BackgroundImage = Resources.O;
                    }
                    else if (GameSettings.Theme == enGameTheme.Neon)
                    {
                        btn.BackgroundImageLayout = ImageLayout.Zoom;
                        btn.BackgroundImage = Resources.ONeon4;
                    }
                    else
                    {
                        btn.BackgroundImage = Resources.OCute;

                    }
                }
            }
        }

        private List<Button> gameButtons;
        private readonly Random rnd = new Random();

        private void PlayRandom()
        {
            var availableButtons = gameButtons
                .Where(b => b.Tag != null && b.Tag.ToString() == "?")
                .ToList();
            
            if (availableButtons.Count == 0)
                return;
            
            int index = rnd.Next(availableButtons.Count);
            Button chosenButton = availableButtons[index];
            
            chosenButton.PerformClick();
        }

/*
        private bool TryWinOrBlock(string symbol)
        {
            int size = (gameButtons.Count == 9) ? 3 : 4;
            Button[,] board = new Button[size, size];

            for (int i = 0; i < size * size; i++)
                board[i / size, i % size] = gameButtons[i];

            for (int r = 0; r < size; r++)
            {
                var row = Enumerable.Range(0, size).Select(c => board[r, c]).ToList();
                if (CheckLine(row, symbol)) return true;
            }

            for (int c = 0; c < size; c++)
            {
                var col = Enumerable.Range(0, size).Select(r => board[r, c]).ToList();
                if (CheckLine(col, symbol)) return true;
            }

            var diag1 = Enumerable.Range(0, size).Select(i => board[i, i]).ToList();
            if (CheckLine(diag1, symbol)) return true;

            var diag2 = Enumerable.Range(0, size).Select(i => board[i, size - 1 - i]).ToList();
            if (CheckLine(diag2, symbol)) return true;

            return false;
        }
        private bool CheckLine(List<Button> line, string symbol)
        {
            var symbols = line.Select(b => b.Text).ToList();
            if (symbols.Count(s => s == symbol) == symbols.Count - 1 &&
                symbols.Count(s => s == "") == 1)
            {
                var target = line.First(b => b.Text == "");
                target.PerformClick();
                return true;
            }
            return false;
        }
*/

        private async void ComputerMove()
        {
  /*
            if (TryWinOrBlock("O")) return;
            
            if (TryWinOrBlock("X")) return;
            System.Threading.Thread.Sleep(500);
  */
            await Task.Delay(500);

            PlayRandom();
        }

        public void ChageImage(Button btn)
        {
            if (btn.Tag.ToString() == "?" && GameStatus.GameOver == false)
            {
                switch (PlayerTurn)
                {
                    case enPlayer.Player1:

                        Buttonimage(btn);
                        PlayerTurn = enPlayer.Player2;
                        lblTurn.Text = tbPlayer2.Text;
                        GameStatus.PlayCount++;
                        btn.Tag = "X";
                        PlayEffect("ChessPieces.wav");
                        CheckWinner();
                        break;
                    case enPlayer.Player2:
                        Buttonimage(btn);
                        PlayerTurn = enPlayer.Player1;
                        lblTurn.Text = tbPlayer1.Text;
                        GameStatus.PlayCount++;
                        btn.Tag = "O";
                        PlayEffect("ChessPieces.wav");
                        CheckWinner();
                        break;
                }
            }
            else
            {
                return;
            }
            
            if (GameSettings.Mode == enGameMode.HumanVsComputer &&
                PlayerTurn == enPlayer.Player2 &&
                GameStatus.GameOver == false)
            {
                ComputerMove();
            }

            int maxMoves = (GameSettings.BoardSize == enBoardSize.ThreeByThree) ? 9 : 16;

            if (GameStatus.PlayCount == maxMoves &&
                GameStatus.Winner != enWinner.Player1 &&
                GameStatus.Winner != enWinner.Player2)
            {
                GameStatus.GameOver = true;
                GameStatus.Winner = enWinner.Draw;
                EndGame();
                
                btnNextRound.Enabled = true;
                btnNextRound.BackColor = Color.Lime;

                rtbGames.Text += System.Environment.NewLine + "Winner of Game "
                    + lblRound.Tag.ToString() + " 🏅 : Draw";
                rtbGames.SelectionStart = rtbGames.Text.Length;
                rtbGames.ScrollToCaret();

                PlayEffect("DrawSound.wav");
            }

        }

        private void ResetButton(Button btn)
        {
            btn.Image = null;
            btn.BackgroundImage = null;
            btn.Tag = "?";
            btn.BackColor = Color.Transparent;
        }

        void RestartGame()
        {
            ResetButton(button1);
            ResetButton(button2);
            ResetButton(button3);
            ResetButton(button4);
            ResetButton(button5);
            ResetButton(button6);
            ResetButton(button7);
            ResetButton(button8);
            ResetButton(button9);
            ResetButton(button12);
            ResetButton(button13);
            ResetButton(button14);
            ResetButton(button15);
            ResetButton(button16);
            ResetButton(button17);
            ResetButton(button18);

            if (Convert.ToByte(lblRound.Tag) % 2 == 0)
            {
                PlayerTurn = enPlayer.Player1;
                lblTurn.Text = tbPlayer1.Text;
            }
            else
            {
                PlayerTurn = enPlayer.Player2;
                lblTurn.Text = tbPlayer2.Text;
            }

            lblRound.Tag = Convert.ToByte(lblRound.Tag) + 1;
            lblRound.Text = "Round " + lblRound.Tag.ToString();

            GameStatus.PlayCount = 0;
            GameStatus.GameOver = false;
            GameStatus.Winner = enWinner.GameInProgress;
            lblWinner.Text = "In Progress";
            btnNextRound.Enabled = false;

            btnNextRound.BackColor = Color.Transparent;

            if (GameSettings.Mode == enGameMode.HumanVsComputer && PlayerTurn == enPlayer.Player2)
            {
                ComputerMove();
            }

        }

        private void btnNextRound_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void button_Click(object sender, EventArgs e)
        {
            ChageImage((Button)sender);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            lblTurn.Text = tbPlayer1.Text;
            lblSocreNamePlayer1.Text = tbPlayer1.Text;
            lblSocreNamePlayer2.Text = tbPlayer2.Text;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            button14.Enabled = true;
            button13.Enabled = true;
            button12.Enabled = true;
            button15.Enabled = true;
            button18.Enabled = true;
            button17.Enabled = true;
            button16.Enabled = true;

            gbCharacter.Enabled = false;
            button10.Enabled = false;
            button10.BackColor = Color.Transparent;

            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            gbGames.Enabled = true;

        }

        private void button11_Click(object sender, EventArgs e)
        {
            PanelMainMenu.Visible = true;
            PanelOptions.Visible = false;
            PanelGame.Visible = false;

            RestartGame();
            button10.Enabled = true;
            gbCharacter.Enabled = true;
            button10.BackColor = Color.Lime;

            rtbGames.Text = null;

            lblRound.Tag = 1;
            lblSocrePlayer1.Text = ":    0";
            lblSocrePlayer2.Text = ":    0";
            lblTurn.Text = " ";

            lblSocrePlayer1.Tag = 0;
            lblSocrePlayer2.Tag = 0;

            lblRound.Text = "Round 1";

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button14.Enabled = false;
            button13.Enabled = false;
            button12.Enabled = false;
            button15.Enabled = false;
            button18.Enabled = false;
            button17.Enabled = false;
            button16.Enabled = false;



            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            gbGames.Enabled = false;

            linkLabel1.LinkVisited = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit the game?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private WaveOutEvent bgOutput;
        private AudioFileReader bgReader;
        private bool bgLooping;

        private void PlayMusicLoop(string fileName)
        {
            string path = Path.Combine(Application.StartupPath, "Sounds", fileName);
            if (!File.Exists(path)) return;

            StopMusic();

            bgReader = new AudioFileReader(path);
            bgOutput = new WaveOutEvent();
            bgOutput.Init(bgReader);
            bgLooping = true;
            bgOutput.PlaybackStopped += BgOutput_PlaybackStopped;
            bgOutput.Play();
        }

        private void BgOutput_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (bgLooping && bgReader != null && bgOutput != null)
            {
                bgReader.Position = 0;
                bgOutput.Play();
            }
        }

        private void StopMusic()
        {
            bgLooping = false;

            if (bgOutput != null)
            {
                bgOutput.PlaybackStopped -= BgOutput_PlaybackStopped;
                bgOutput.Stop();
                bgOutput.Dispose();
                bgOutput = null;
            }

            if (bgReader != null)
            {
                bgReader.Dispose();
                bgReader = null;
            }
        }

        private void PlayEffect(string fileName)
        {
            string path = Path.Combine(Application.StartupPath, "Sounds", fileName);
            if (!File.Exists(path)) return;

            var reader = new AudioFileReader(path);
            var output = new WaveOutEvent();
            output.Init(reader);
            output.Play();
            output.PlaybackStopped += (s, e) =>
            {
                output.Dispose();
                reader.Dispose();
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopMusic();
            base.OnFormClosing(e);
        }

        private bool isMuted = false;

        private void btnMuteMusic_Click(object sender, EventArgs e)
        {
            if (!isMuted)
            {
                StopMusic();
                isMuted = true;
                btnMuteMusic.Text = "🔇";
            }
            else
            {
                PlayMusicLoop("ClassicMusic.mp3");
                isMuted = false;
                btnMuteMusic.Text = "♪";
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.linkedin.com/in/shehab-ghitany",
                UseShellExecute = true
            });
            linkLabel1.LinkVisited = true;
        }
    }
}
