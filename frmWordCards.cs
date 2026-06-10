using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace WordCards
{
    public partial class frmWordCards : Form
    {
        /// <summary>
        /// 單字清單
        /// </summary>
        private readonly WordCollection _WordList = new WordCollection();

        /// <summary>
        /// Windows Media Player 播放器
        /// </summary>
        private readonly WindowsMediaPlayer wmp = new WindowsMediaPlayer();

        private string strWordFile = "WordCards.txt";

        /// <summary>
        /// 是否自動播放
        /// </summary>
        private bool isPlay = false;

        /// <summary>
        /// 關於視窗
        /// </summary>
        private readonly frmAbout about = new frmAbout();

        public frmWordCards()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 顯示單字。
        /// </summary>
        private void ShowWord(WordItem word)
        {
            if (word == null)
            {
                txtWord.Text = string.Empty;
                txtPhonogram.Text = string.Empty;
                txtExplain.Text = string.Empty;
                return;
            }

            txtWord.Text = word.Word;
            txtPhonogram.Text = word.Phonogram;
            txtExplain.Text = word.Explain;
        }

        /// <summary>
        /// 更新單字列表。
        /// </summary>
        private void UpdateWordList()
        {
            lstWordList.BeginUpdate();
            lstWordList.Items.Clear();

            foreach (WordItem item in _WordList)
            {
                lstWordList.Items.Add(item);
            }

            lstWordList.EndUpdate();
        }

        /// <summary>
        /// 從檔案載入單字資料。
        /// </summary>
        private void LoadWordFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"找不到單字檔\n{filePath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            _WordList.LoadFromStringArray(lines);
            strWordFile = filePath;
            UpdateWordList();

            if (_WordList.Count > 0)
            {
                lstWordList.SelectedIndex = 0;
                ShowWord(_WordList[0]);
                tsslMessage.Text = $"單字數量：{_WordList.Count}";
            }
            else
            {
                ShowWord(null);
                tsslMessage.Text = "單字檔沒有資料";
            }
        }

        /// <summary>
        /// 播放單字音檔。
        /// </summary>
        public void PlayWord(WordItem word)
        {
            if (word == null || string.IsNullOrWhiteSpace(word.SoundPath))
            {
                tsslMessage.Text = "此單字沒有音效路徑";
                return;
            }

            string soundPath = word.SoundPath;
            if (!Path.IsPathRooted(soundPath))
            {
                soundPath = Path.Combine(Application.StartupPath, soundPath);
            }

            if (!File.Exists(soundPath))
            {
                tsslMessage.Text = $"找無 {word.SoundPath} 音效檔";
                return;
            }

            try
            {
                wmp.controls.stop();
                wmp.URL = soundPath;
                wmp.settings.autoStart = false;
                wmp.settings.mute = false;
                wmp.controls.play();
                tsslMessage.Text = $"播放：{word.Word}";
            }
            catch (Exception ex)
            {
                tsslMessage.Text = "音效播放失敗：" + ex.Message;
            }
        }

        /// <summary>
        /// 播放目前選取的單字。
        /// </summary>
        private void PlaySelectedWord()
        {
            if (lstWordList.SelectedIndex >= 0 && lstWordList.SelectedIndex < _WordList.Count)
            {
                int idx = lstWordList.SelectedIndex;
                ShowWord(_WordList[idx]);
                PlayWord(_WordList[idx]);
            }
        }

        /// <summary>
        /// 將單字清單的選項移到下一個。
        /// </summary>
        private void NextWordList()
        {
            if (lstWordList.Items.Count == 0)
                return;

            lstWordList.Focus();

            if (lstWordList.SelectedIndex < 0 || lstWordList.SelectedIndex + 1 >= lstWordList.Items.Count)
                lstWordList.SelectedIndex = 0;
            else
                lstWordList.SelectedIndex++;

            try
            {
                int itemHeight = Math.Max(1, lstWordList.GetItemHeight(0));
                int lstRows = Math.Max(1, lstWordList.Height / itemHeight);
                if (lstWordList.SelectedIndex >= lstRows / 2)
                    lstWordList.TopIndex = Math.Max(0, lstWordList.SelectedIndex - lstRows / 2);
            }
            catch
            {
                // ListBox 尚未完成繪製時不影響主要功能。
            }
        }

        private void frmWordCards_Load(object sender, EventArgs e)
        {
            BuildLogo();
            string startupFile = Path.Combine(Application.StartupPath, strWordFile);
            LoadWordFile(startupFile);
        }

        private void BuildLogo()
        {
            Bitmap bmp = new Bitmap(picLogo.Width, picLogo.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(255, 254, 242));
                using (Brush bg = new SolidBrush(Color.FromArgb(255, 245, 180)))
                    g.FillEllipse(bg, 14, 10, 58, 58);
                using (Pen pen = new Pen(Color.FromArgb(64, 64, 64), 2))
                    g.DrawEllipse(pen, 14, 10, 58, 58);
                using (Font f = new Font("Segoe UI", 22, FontStyle.Bold))
                using (Brush b = new SolidBrush(Color.HotPink))
                    g.DrawString("A", f, b, 26, 18);
                using (Font f2 = new Font("Microsoft JhengHei", 9, FontStyle.Bold))
                using (Brush b2 = new SolidBrush(Color.FromArgb(64, 64, 64)))
                    g.DrawString("單字卡", f2, b2, 18, 74);
            }
            picLogo.Image = bmp;
        }

        private void lstWordList_Click(object sender, EventArgs e)
        {
            if (isPlay)
                btnAutoPlay.PerformClick();

            if (lstWordList.SelectedItem != null && lstWordList.SelectedItem.ToString().Length != 0)
                PlaySelectedWord();
        }

        private void timPlayer_Tick(object sender, EventArgs e)
        {
            NextWordList();
            PlaySelectedWord();
        }

        private void btnAutoPlay_Click(object sender, EventArgs e)
        {
            lstWordList.Focus();

            if (_WordList.Count == 0)
            {
                tsslMessage.Text = "沒有單字可以播放";
                return;
            }

            if (isPlay == false)
            {
                btnAutoPlay.Text = "Stop";
                isPlay = true;

                if (lstWordList.SelectedIndex < 0)
                    lstWordList.SelectedIndex = 0;

                PlaySelectedWord();
                timPlayer.Start();
            }
            else
            {
                btnAutoPlay.Text = "Play";
                isPlay = false;
                timPlayer.Stop();
                tsslMessage.Text = "自動播放已停止";
            }
        }

        private void frmWordCards_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isPlay == true)
                return;

            switch (e.KeyChar)
            {
                case (char)Keys.Return:
                    NextWordList();
                    PlaySelectedWord();
                    e.Handled = true;
                    break;

                case (char)Keys.Space:
                    if (lstWordList.SelectedIndex >= 0)
                        PlaySelectedWord();
                    e.Handled = true;
                    break;
            }
        }

        private void lstWordList_DoubleClick(object sender, EventArgs e)
        {
            if (lstWordList.SelectedIndex < 0 || lstWordList.SelectedIndex >= _WordList.Count)
                return;

            lstWordList.Focus();
            int idx = lstWordList.SelectedIndex;

            using (frmEditWord edit = new frmEditWord(_WordList[idx]))
            {
                DialogResult result = edit.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    UpdateWordList();
                    lstWordList.SelectedIndex = idx;
                    PlaySelectedWord();
                    _WordList.SaveToFile(strWordFile);
                    tsslMessage.Text = "單字已修改並儲存";
                }
            }
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "TSV files (*.tsv)|*.tsv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.Title = "開啟單字檔";
                ofd.InitialDirectory = Application.StartupPath;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                    LoadWordFile(ofd.FileName);
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            about.ShowDialog(this);
        }

        private void frmWordCards_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;

            DialogResult dr = MessageBox.Show("確定要離開嗎?", "離開", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
                e.Cancel = true;
        }
    }
}
