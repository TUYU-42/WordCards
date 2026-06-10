using System;
using System.Windows.Forms;

namespace WordCards
{
    public partial class frmEditWord : Form
    {
        public WordItem Word { get; set; }

        public frmEditWord(WordItem word)
        {
            InitializeComponent();
            Word = word;

            if (word != null)
            {
                txtWord.Text = word.Word;
                txtPhonogram.Text = word.Phonogram;
                txtSoundPath.Text = word.SoundPath;
                txtExplain.Text = word.Explain;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Word == null)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWord.Text))
            {
                MessageBox.Show("單字不可為空白。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWord.Focus();
                return;
            }

            Word.Word = txtWord.Text.Trim();
            Word.Phonogram = txtPhonogram.Text.Trim();
            Word.SoundPath = txtSoundPath.Text.Trim();
            Word.Explain = txtExplain.Text.Trim();

            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
