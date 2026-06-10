using System;
using System.Linq;

namespace WordCards
{
    /// <summary>
    /// 單一單字資料。
    /// TSV 欄位格式：Word、Phonogram、SoundPath、Explain...
    /// </summary>
    public class WordItem
    {
        public string Word { get; set; }
        public string Phonogram { get; set; }
        public string SoundPath { get; set; }
        public string Explain { get; set; }

        public WordItem()
        {
            Word = string.Empty;
            Phonogram = string.Empty;
            SoundPath = string.Empty;
            Explain = string.Empty;
        }

        /// <summary>
        /// 建構子：由一行 TSV 資料建立 WordItem。
        /// </summary>
        /// <param name="str">單行的單字資料</param>
        public WordItem(string str) : this()
        {
            if (str == null)
                return;

            string[] strLists = str.Split('\t');
            Word = strLists.Length > 0 ? strLists[0].Trim() : string.Empty;
            Phonogram = strLists.Length > 1 ? strLists[1].Trim() : string.Empty;
            SoundPath = strLists.Length > 2 ? strLists[2].Trim() : string.Empty;

            if (strLists.Length > 3)
            {
                Explain = string.Join(Environment.NewLine, strLists.Skip(3).Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }

        /// <summary>
        /// 覆寫 ToString()，讓 ListBox 自動顯示單字。
        /// </summary>
        public override string ToString()
        {
            return Word;
        }

        /// <summary>
        /// 將 WordItem 轉回 TSV 的一行，供存檔使用。
        /// </summary>
        public string ToLineString()
        {
            string safeExplain = Explain ?? string.Empty;
            safeExplain = safeExplain.Replace("\r\n", "\t").Replace("\n", "\t").Replace("\r", "\t");
            return $"{Word}\t{Phonogram}\t{SoundPath}\t{safeExplain}";
        }
    }
}
