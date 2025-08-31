using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane
{
    public static class Converter
    {
        /// <summary>
        /// 36進数パーサー
        /// </summary>
        public static bool TryParseBase36(string s, out int result)
        {
            result = 0;
            if (string.IsNullOrEmpty(s)) return false;

            s = s.ToUpperInvariant();
            foreach (var c in s)
            {
                int val;
                if (c >= '0' && c <= '9') val = c - '0';
                else if (c >= 'A' && c <= 'Z') val = c - 'A' + 10;
                else return false; // 0-9/A-Z以外は不可

                result = result * 36 + val;
            }
            return true;
        }


        /// <summary>
        /// int → 36進数文字列に変換
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="digit">出力桁数</param>
        /// <returns>36進数文字列</returns>
        public static string IntToBase36(int value, int digit)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            string s = "";
            do
            {
                s = chars[value % 36] + s;
                value /= 36;
            } while (value > 0);

            return s.PadLeft(digit, '0');
        }
    }
}
