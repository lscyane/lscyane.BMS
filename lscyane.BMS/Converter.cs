using System;
using System.Collections.Generic;
using System.Text;

namespace lscyane.BMS
{
    public static class Converter
    {
        /// <summary>
        /// 汎用 数値から文字列への変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseNum"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string IntToString(int value, int baseNum, int digit = 0)
        {
            var retval = baseNum switch
            {
                10 => value.ToString(),
                16 => value.ToString("X"),
                36 => IntToBase36(value, digit),
                _ => throw new ArgumentOutOfRangeException()
            };
            retval = retval.PadLeft(digit, '0');

            return retval;
        }


        /// <summary>
        /// 36進数パーサー
        /// </summary>
        public static int ParseBase36(string s)
        {
            var result = 0;
            if (string.IsNullOrEmpty(s)) throw new FormatException();

            s = s.ToUpperInvariant();
            foreach (var c in s)
            {
                int val;
                if (c >= '0' && c <= '9') val = c - '0';
                else if (c >= 'A' && c <= 'Z') val = c - 'A' + 10;
                else throw new FormatException(); // 0-9/A-Z以外は不可

                result = result * 36 + val;
            }
            return result;
        }


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
