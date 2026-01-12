using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane.BMS
{
    /// <summary>
    /// 出力オプション
    /// </summary>
    public class OutputOption
    {
        /// <summary>
        /// 出力ファイルの文字コード (規定値はShift_JIS)
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.GetEncoding("Shift_JIS");


        /// <summary>
        /// #TITLE などのヘッダ類の分割記号 コロンやスペースが一般的 (規定値はコロン)
        /// </summary>
        public string HeaderSplitter { get; set; } = ":";


        /// <summary>
        /// #WAV などの定義系の分割記号 コロンやスペースが一般的 (規定値はコロン)
        /// </summary>
        public string DefinitionSplitter { get; set; } = ":";
    }
}
