using System;
using System.Collections.Generic;

namespace lscyane.BMS
{
    public class Note
    {
        /// <summary> チャンネル番号 </summary>
        public string Channel { get; set; } = "00";

        /// <summary> 小節番号 </summary>
        public int Measure { get; set; }

        /// <summary> ノートの値 </summary>
        public string Value { get; set; } = "00";

        /// <summary> 分子 </summary>
        public int Numerator { get; set; }

        /// <summary> 分母 </summary>
        public int Denominator { get; set; }


        /// <summary>
        /// BPMと拍位置からノートのタイミング[sec]を算出する
        /// </summary>
        /// <param name="bpm"></param>
        /// <returns></returns>
        public double GetTime(double bpm)
        {
            if (bpm <= 0) return 0;
            var beatDuration = 60000.0 / bpm;                                   // 1拍の長さ[ms]
            var positionInBeats = (double)this.Numerator / this.Denominator;    // 拍位置(4分音符を1拍とする)
            var notePosition = positionInBeats + (this.Measure * 4);            // ノートのタイミング[拍ベース]
            var timeInMs = beatDuration * notePosition;                         // ノートのタイミング[ms]
            return timeInMs / 1000.0;   // 秒に換算して返す
        }


        /// <summary>
        /// 小節内の Tick 値（分数を自然数化）
        /// </summary>
        public double BeatPosition => (double)this.Numerator / this.Denominator;


        /// <summary>
        /// ソート用にBPM変化を考慮しない大体の位置を返す
        /// </summary>
        public double AboutPosition => this.Measure + this.BeatPosition;
    }
}
