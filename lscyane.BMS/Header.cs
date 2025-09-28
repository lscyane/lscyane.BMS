using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane.BMS
{
    public class Header
    {
        /// <summary> タイトル </summary>
        public string TITLE { get; set; } = string.Empty;

        /// <summary> アーティスト名 </summary>
        public string ARTIST { get; set; } = string.Empty;

        /// <summary> サブアーティスト名 </summary>
        public string SUBARTIST { get; set; } = string.Empty;

        /// <summary> コメント </summary>
        public string COMMENT { get; set; } = string.Empty;

        /// <summary>  </summary>
        public string PANEL { get; set; } = string.Empty;

        /// <summary> 選曲プレビュー音楽 </summary>
        public string PREVIEW { get; set; } = string.Empty;

        /// <summary> 選曲ジャケット画像 </summary>
        public string PREIMAGE { get; set; } = string.Empty;

        /// <summary> 読込中画像 </summary>
        public string STAGEFILE { get; set; } = string.Empty;

        /// <summary> 背景画像 </summary>
        public string BACKGROUND { get; set; } = string.Empty;

        /// <summary> リザルト画像 </summary>
        public string RESULTIMAGE { get; set; } = string.Empty;

        /// <summary> BPM 初期値 </summary>
        public decimal BPM { get; set; }

        /// <summary> ドラム難易度 </summary>
        public string DLEVEL { get; set; } = string.Empty;

        /// <summary> ギター難易度 </summary>
        public string GLEVEL { get; set; } = string.Empty;

        /// <summary> ベース難易度 </summary>
        public string BLEVEL { get; set; } = string.Empty;

        /// <summary> ジャンル </summary>
        public string GENRE { get; set; } = string.Empty;

        /// <summary>  </summary>
        public int PLAYLEVEL { get; set; }

        /// <summary>  </summary>
        public int RANK { get; set; }

        /// <summary>  </summary>
        public int PLAYER { get; set; }

        /// <summary>  </summary>
        public int TOTAL { get; set; }

        /// <summary> 乱数処理 </summary>
        public int Random { get; set; }

    }
}
