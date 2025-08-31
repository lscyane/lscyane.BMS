using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane
{
    public class Note
    {
        /// <summary> チャンネル番号 </summary>
        public int Channel { get; set; }

        /// <summary> 小説番号 </summary>
        public int Measure { get; set; }

        /// <summary> ノートの値 </summary>
        public int Value { get; set; }

        /// <summary> 分子 </summary>
        public int Numerator { get; set; }

        /// <summary> 分母 </summary>
        public int Denominator { get; set; }

        /// <summary>
        /// BPMと拍位置からノートのタイミング[ms]を算出する
        /// </summary>
        /// <param name="bpm"></param>
        /// <returns></returns>
        public decimal GetTime(decimal bpm)
        {
            if (bpm <= 0) return 0;
            var beatDuration = 60000m / bpm;                        // 1拍の長さ[ms]
            var positionInBeats = (decimal)Numerator / Denominator; // 拍位置(4分音符を1拍とする)
            var timeInMs = beatDuration * positionInBeats;          // ノートのタイミング[ms]
            return timeInMs;
        }
    }
}
