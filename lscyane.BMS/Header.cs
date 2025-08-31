using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane
{
    public class Header
    {
        /// <summary> タイトル </summary>
        public string TITLE { get; set; } = string.Empty;

        /// <summary> アーティスト名 </summary>
        public string ARTIST { get; set; } = string.Empty;

        /// <summary>  </summary>
        public string STAGEFILE { get; set; } = string.Empty;

        /// <summary>  </summary>
        public string DLEVEL { get; set; } = string.Empty;

        /// <summary>  </summary>
        public string GENRE { get; set; } = string.Empty;

        /// <summary>  </summary>
        public int PLAYLEVEL { get; set; }

        /// <summary>  </summary>
        public int RANK { get; set; }

        /// <summary>  </summary>
        public int PLAYER { get; set; }

        /// <summary>  </summary>
        public int TOTAL { get; set; }

        /// <summary>  </summary>
        public decimal BPM { get; set; }
    }
}
