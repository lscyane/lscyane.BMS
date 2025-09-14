using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane.BMS
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
        public double GetTime(double bpm)
        {
            if (bpm <= 0) return 0;
            var beatDuration = 60000.0 / bpm;                        // 1拍の長さ[ms]
            var positionInBeats = (double)Numerator / Denominator; // 拍位置(4分音符を1拍とする)
            var timeInMs = beatDuration * positionInBeats;          // ノートのタイミング[ms]
            return timeInMs;
        }

        /// <summary>
        /// 小節内の Tick 値（分数を整数化）
        /// </summary>
        public double BeatPosition => (double)Numerator / Denominator;

        /// <summary>
        /// 時間軸上の絶対位置（小節番号＋小節内位置）
        /// </summary>
        public double AbsolutePosition => Measure + BeatPosition;
    }
}
