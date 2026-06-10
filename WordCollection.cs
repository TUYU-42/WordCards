using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace WordCards
{
    /// <summary>
    /// 自訂單字集合，負責載入與儲存 WordItem。
    /// </summary>
    public class WordCollection : Collection<WordItem>
    {
        /// <summary>
        /// 從字串陣列載入資料。
        /// </summary>
        /// <param name="lines">輸入的單字字串陣列</param>
        public void LoadFromStringArray(string[] lines)
        {
            Clear();

            if (lines == null)
                return;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                WordItem item = new WordItem(line);
                if (!string.IsNullOrWhiteSpace(item.Word))
                    Add(item);
            }
        }

        /// <summary>
        /// 將單字集合儲存回 TSV 文字檔。
        /// </summary>
        public void SaveToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("檔案路徑不可為空白。", nameof(filePath));

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (WordItem item in this)
                {
                    writer.WriteLine(item.ToLineString());
                }
            }
        }
    }
}
