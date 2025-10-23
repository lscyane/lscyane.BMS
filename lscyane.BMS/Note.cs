using System;
using System.Collections.Generic;

namespace lscyane.BMS
{
    /// <summary>
    /// ノートクラス
    /// </summary>
    public class Note
    {
        /// <summary>
        /// チャンネル番号
        /// </summary>
        /// <remarks>
        /// BMSのBGMなど、同じチャンネルを複数定義する場合があるので、それを管理するための値
        /// </remarks>
        public string Channel { get; set; } = "00";

        /// <summary> ノートの値 </summary>
        public string Value { get; set; } = "00";

        /// <summary> 小節番号 </summary>
        public int Measure { get; set; }

        /// <summary> 分子 </summary>
        public int Numerator { get; set; }

        /// <summary> 分母 </summary>
        public int Denominator { get; set; }


        /// <summary>
        /// 小節内の Tick 値（分数を自然数化）
        /// </summary>
        public double BeatPosition => (double)this.Numerator / this.Denominator;


        /// <summary>
        /// BPM変化や小節拡大を考慮しない論理的な小節位置を返す
        /// </summary>
        public double MeasurePosition => this.Measure + this.BeatPosition;


        /// <summary>
        /// 小節位置（分数形式）
        /// </summary>
        public FractionalPosition MeasurePositionFrac
        {
            get => new FractionalPosition(this.Measure, this.Numerator, this.Denominator);
            set
            {
                this.Measure = value.Measure;
                this.Numerator = value.Numerator;
                this.Denominator = value.Denominator;
            }
        }
    }


    #region FractionalPosition 構造体
    /// <summary>
    /// 分数形式で小節位置を表す構造体
    /// </summary>
    public struct FractionalPosition : IEquatable<FractionalPosition>, IComparable<FractionalPosition>
    {
        public int Measure;     // 小節番号
        public int Numerator;   // 分子
        public int Denominator; // 分母

        public FractionalPosition(int measure, int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException("Denominator cannot be zero.");
            }
            this.Measure = measure;
            this.Numerator = numerator;
            this.Denominator = denominator;
            this.NormalizeSelf();
        }


        /// <summary>
        /// 約分して正規化（符号と約分）
        /// </summary>
        private void NormalizeSelf()
        {
            if (this.Denominator < 0)
            {
                this.Numerator = -this.Numerator;
                this.Denominator = -this.Denominator;
            }

            int g = Gcd(Math.Abs(this.Numerator), this.Denominator);
            if (g > 1)
            {
                this.Numerator /= g;
                this.Denominator /= g;
            }
        }


        /// <summary>
        /// 最大公約数（Greatest Common Divisor） を求める関数
        /// </summary>
        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int t = a % b;
                a = b;
                b = t;
            }
            return a;
        }

        // --- 等価比較 ---
        public bool Equals(FractionalPosition other)
        {
            return this.Measure == other.Measure &&
                    this.Numerator * other.Denominator == other.Numerator * this.Denominator;
        }
        public override bool Equals(object obj)
        {
            return obj is FractionalPosition other && Equals(other);
        }


        public override int GetHashCode()
        {
            // 分数部分を約分してからハッシュ化
            var n = new FractionalPosition(this.Measure, this.Numerator, this.Denominator);
            return this.Measure.GetHashCode() ^ n.Numerator.GetHashCode() ^ n.Denominator.GetHashCode();
        }


        public static bool operator ==(FractionalPosition a, FractionalPosition b) => a.Equals(b);
        public static bool operator !=(FractionalPosition a, FractionalPosition b) => !a.Equals(b);


        // --- 大小比較 ---
        public int CompareTo(FractionalPosition other)
        {
            if (this.Measure != other.Measure)
            {
                return Measure.CompareTo(other.Measure);
            }
            long lhs = (long)this.Numerator * other.Denominator;
            long rhs = (long)other.Numerator * this.Denominator;
            return lhs.CompareTo(rhs);
        }

        public static bool operator <(FractionalPosition a, FractionalPosition b) => a.CompareTo(b) < 0;
        public static bool operator >(FractionalPosition a, FractionalPosition b) => a.CompareTo(b) > 0;
        public static bool operator <=(FractionalPosition a, FractionalPosition b) => a.CompareTo(b) <= 0;
        public static bool operator >=(FractionalPosition a, FractionalPosition b) => a.CompareTo(b) >= 0;


        // --- 加算・減算 ---
        public static FractionalPosition operator +(FractionalPosition a, FractionalPosition b)
        {
            // 拍位置の合計
            int m = a.Measure + b.Measure;
            long n = (long)a.Numerator * b.Denominator + (long)b.Numerator * a.Denominator;
            long d = (long)a.Denominator * b.Denominator;

            // 小節を繰り上げ
            if (n >= d)
            {
                m += (int)(n / d);
                n %= d;
            }

            return new FractionalPosition(m, (int)n, (int)d);
        }


        public static FractionalPosition operator -(FractionalPosition a, FractionalPosition b)
        {
            int m = a.Measure - b.Measure;
            long n = (long)a.Numerator * b.Denominator - (long)b.Numerator * a.Denominator;
            long d = (long)a.Denominator * b.Denominator;

            // 負の拍を補正（小節繰り下げ）
            while (n < 0)
            {
                n += d;
                m--;
            }

            return new FractionalPosition(m, (int)n, (int)d);
        }


        public override string ToString()
        {
            return string.Format("{0}+{1}/{2}", this.Measure, this.Numerator, this.Denominator);
        }
    }
    #endregion
    
}
