using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane.BMS
{
    /// <summary>
    /// WAV や BMP などのデータを表すクラス
    /// </summary>
    public class Table
    {
        /// <summary> ファイルパス </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary> コメント </summary>
        public string Comment { get; set; } = string.Empty;


        public Table(string data)
        {
            var parts = data.Split(';', 2);
            this.Data = parts[0].Trim();
            if (parts.Length > 1)
            {
                this.Comment = parts[1];
            }
        }
    }
}
