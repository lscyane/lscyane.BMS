using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace lscyane.BMS
{
    /// <summary>
    /// BMS形式クラス
    /// </summary>
    public class Format
    {
        /// <summary> ヘッダ </summary>
        public Header Header { get; } = new Header();

        /// <summary> WAV定義 </summary>
        public Dictionary<string, Table> WAV { get; } = new Dictionary<string, Table>();
        /// <summary> BMP定義 </summary>
        public Dictionary<string, Table> BMP { get; } = new Dictionary<string, Table>();
        /// <summary> BPM定義 </summary>
        public Dictionary<string, double> BPMDefs { get; } = new Dictionary<string, double>();
        /// <summary> STOP定義 </summary>
        public Dictionary<int, int> STOP { get; } = new Dictionary<int, int>();

        /// <summary> 小節の短縮 (小節拡大率) </summary> <remarks> Key:小節番号 Value:拡大率 </remarks>
        public Dictionary<int, double> BarMagnification { get; } = new Dictionary<int, double>();

        /// <summary> ノート </summary>
        public List<Note> Notes { get; } = new List<Note>();


        #region テンポラリ変数
        int beforeMeasure = -1;
        int subChNum = 0;
        #endregion


        /// <summary>
        /// BMSファイルを書き出す
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path, Encoding? enc = null)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (enc == null)
            {
                enc = Encoding.GetEncoding("Shift_JIS");
            }

            using var sw = new System.IO.StreamWriter(path, false, enc);

            // --------------------------
            // ヘッダ出力
            // --------------------------
            void WriteHeader(string key, object value)
            {
                if (value == null) return;
                sw.WriteLine($"{key}: {value}");
            }
            WriteHeader("#TITLE", Header.TITLE);
            WriteHeader("#ARTIST", Header.ARTIST);
            WriteHeader("#COMMENT", Header.COMMENT);
            WriteHeader("#PANEL", Header.PANEL);
            WriteHeader("#PREVIEW", Header.PREVIEW);
            WriteHeader("#PREIMAGE", Header.PREIMAGE);
            WriteHeader("#STAGEFILE", Header.STAGEFILE);
            WriteHeader("#BACKGROUND", Header.BACKGROUND);
            WriteHeader("#RESULTIMAGE", Header.RESULTIMAGE);
            WriteHeader("#BPM", Header.BPM);
            WriteHeader("#DLEVEL", Header.DLEVEL);
            WriteHeader("#GLEVEL", Header.GLEVEL);
            WriteHeader("#BLEVEL", Header.BLEVEL);
            sw.WriteLine("");
            WriteHeader("#GENRE", Header.GENRE);
            WriteHeader("#PLAYLEVEL", Header.PLAYLEVEL);
            WriteHeader("#RANK", Header.RANK);
            WriteHeader("#PLAYER", Header.PLAYER);
            WriteHeader("#TOTAL", Header.TOTAL);
            sw.WriteLine("");

            // --------------------------
            // 定義系出力
            // --------------------------
            foreach (var kv in this.BarMagnification.OrderBy(k => k.Key))
            {
                sw.WriteLine($"#{kv.Key:D03}02: {kv.Value}");
            }
            foreach (var kv in WAV.OrderBy(k => k.Key))
            {
                var comment = string.IsNullOrEmpty(kv.Value.Comment) ? "" : $"\t;{kv.Value.Comment}";
                sw.WriteLine($"#WAV{kv.Key}: {kv.Value.Data}{comment}");
            }
            sw.WriteLine("");
            foreach (var kv in BMP.OrderBy(k => k.Key))
            {
                var comment = string.IsNullOrEmpty(kv.Value.Comment) ? "" : $"\t;{kv.Value.Comment}";
                sw.WriteLine($"#BMP{kv.Key, 2}: {kv.Value}");
            }
            sw.WriteLine("");
            foreach (var kv in BPMDefs.OrderBy(k => k.Key))
            {
                sw.WriteLine($"#BPM{kv.Key, 2}: {kv.Value}");
            }
            sw.WriteLine("");
            foreach (var kv in STOP.OrderBy(k => k.Key))
            {
                sw.WriteLine($"#STOP{Converter.IntToBase36(kv.Key, 2)}: {kv.Value}");
            }

            // --------------------------
            // ノート出力（小節・チャンネル・Denominatorごとにグループ化）
            // --------------------------
            var notesByMeasureChannelDen = this.Notes
                // 小節番号、チャンネル番号、Denominatorごとにグループ化
                .GroupBy(n => new { n.Measure, n.Channel, n.Denominator })
                // 小節番号→チャンネル番号→Denominatorの順でソート
                .OrderBy(g => g.Key.Measure)
                .ThenBy(g => g.Key.Channel)
                .ThenBy(g => g.Key.Denominator);

            foreach (var group in notesByMeasureChannelDen)
            {
                int measure = group.Key.Measure;            // 小節番号
                var channel = group.Key.Channel;            // チャンネル番号
                int denominator = group.Key.Denominator;    // 分母（小節内の分割数）

                // 小節・チャンネル・Denominatorごとのノート一覧をNumerator順に並べ替え
                var notesInGroup = group.OrderBy(n => n.Numerator).ToList();

                // 出力用の文字列を作成（'0'で初期化、各ノートが2文字ずつ使用）
                char[] data = new string('0', denominator * 2).ToCharArray();

                foreach (var note in notesInGroup)
                {
                    // Numeratorの位置に2文字分を埋め込む
                    data[note.Numerator * 2] = note.Value[0];
                    data[note.Numerator * 2 + 1] = note.Value[1];
                }

                // 出力例: #0010A:00001700...
                // 小節番号3桁＋チャンネル2桁（36進数）＋:＋データ列
                sw.WriteLine($"#{measure:000}{channel}:{new string(data)}");
            }
        }


        /// <summary>
        /// BMSファイルを読み込む
        /// </summary>
        public static Format Load(string path, Encoding? enc = null)
        {
            var bms = new Format();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (enc == null)
            {
                enc = Encoding.GetEncoding("Shift_JIS");
            }

            var sr = new System.IO.StreamReader(path, enc);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("#"))
                    continue;

                // ヘッダ系の判定
                if (ParseHeader(bms, line))
                    continue;

                // 定義系の判定
                if (ParseDefinition(bms, line))
                    continue;

                // ノート系の判定
                if (ParseNotes(bms, line))
                    continue;

                // ここまで来たら不明な行
                System.Diagnostics.Debug.WriteLine("[Warning] 不明な行をスキップしました");
                System.Diagnostics.Debug.WriteLine("       -> " + line);
            }

            return bms;
        }


        /// <summary>
        ///  ヘッダ系の判定
        /// </summary>
        /// <returns>true:ヘッダ  false:ヘッダではなかった</returns>
        static bool ParseHeader(Format bms, string line)
        {
            string key, value;
            if (line.Contains(":"))
            {
                // コロン区切り (#TITLE:曲名)
                var parts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return false;
                key = parts[0].Trim().ToUpperInvariant();
                value = parts[1].Trim();
            }
            else
            {
                // スペース区切り (#TITLE 曲名)
                var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return false;
                key = parts[0].Trim().ToUpperInvariant();
                value = parts[1].Trim();
            }

            // ヘッダ情報を設定
            switch (key.ToUpper())
            {
                case "#TITLE": bms.Header.TITLE = value; break;
                case "#ARTIST": bms.Header.ARTIST = value; break;
                case "#SUBARTIST": bms.Header.SUBARTIST = value; break;
                case "#COMMENT": bms.Header.COMMENT = value; break;
                case "#PANEL": bms.Header.PANEL = value; break;
                case "#PREVIEW": bms.Header.PREVIEW = value; break;
                case "#PREIMAGE": bms.Header.PREIMAGE = value; break;
                case "#STAGEFILE": bms.Header.STAGEFILE = value; break;
                case "#BACKGROUND": bms.Header.BACKGROUND = value; break;
                case "#RESULTIMAGE": bms.Header.RESULTIMAGE = value; break;
                case "#BPM": bms.Header.BPM = decimal.TryParse(value, out var bpm) ? bpm : 0; break;
                case "#DLEVEL": bms.Header.DLEVEL = value; break;
                case "#GLEVEL": bms.Header.GLEVEL = value; break;
                case "#BLEVEL": bms.Header.BLEVEL = value; break;
                case "#GENRE": bms.Header.GENRE = value; break;
                case "#PLAYLEVEL": bms.Header.PLAYLEVEL = int.TryParse(value, out var plv) ? plv : 0; break;
                case "#RANK": bms.Header.RANK = int.TryParse(value, out var rnk) ? rnk : 0; break;
                case "#PLAYER": bms.Header.PLAYER = int.TryParse(value, out var ply) ? ply : 0; break;
                case "#TOTAL": bms.Header.TOTAL = int.TryParse(value, out var tot) ? tot : 0; break;
                case "#RANDOM": /* TODO */ break;
                case "#IF": /* TODO */ break;
                case "#ENDIF": /* TODO */ break;

                // 対応予定なし
                case "#BGMWAV":
                    // 古くからのBMSプレイヤーは全て対応しているが、最近の「キー音必須」文化ではほぼ使われない。
                    // キー音文化が根付く前のBMSに多用されたが、今は非推奨気味
                    // 曲開始時に自動再生・常にループ・停止は不可
                    System.Diagnostics.Debug.WriteLine($"非対応のBMSヘッダ : {key} - {value}");
                    break;
                default:    return false;   // 処理可能なヘッダではない
            }

            return true;
        }


        /// <summary>
        /// 定義系の判定
        /// </summary>
        /// <returns>true:定義系  false:定義系ではなかった</returns>
        static bool ParseDefinition(Format bms, string line)
        {
            // 定義系 (#WAV01 xxx.wav)
            if (!line.Contains(" ")) return false;
            var parts = line.Replace('\t',' ').Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return false;
            var key = parts[0].Trim().ToUpperInvariant();
            var value = parts[1].Trim();
            if (key.StartsWith("#WAV"))
            {
                var idStr = key.Substring(4,2); // "01" 部分
                bms.WAV[idStr] = new Table(value);
                return true;
            }
            else if (key.StartsWith("#BMP"))
            {
                var idStr = key.Substring(4,2); // "01" 部分
                bms.BMP[idStr] = new Table(value);
                return true;
            }
            else if (key.StartsWith("#BPM"))
            {
                var idStr = key.Substring(4,2); // "01" 部分
                if (!double.TryParse(value, out var bpmValue))
                    return false;
                bms.BPMDefs[idStr] = bpmValue;
                return true;
            }
            return false; // 処理可能な定義ではない
        }


        /// <summary>
        /// ノート系の判定
        /// </summary>
        /// <returns>true:ノート系  false:ノート系ではなかった</returns>
        static bool ParseNotes(Format bms, string line)
        {
            // ノート系 (#<小節番号><チャンネル>:<データ列>)
            var header = line.Substring(1, 5); // "00111" 部分
            if (!int.TryParse(header.Substring(0, 3), out var measure)) return false;

            // エラーチェック : チャンネル番号の確認
            var channel = header.Substring(3, 2);
            if (channel.Length != 2)
            {
                System.Diagnostics.Debug.WriteLine("[Warning] チャンネル番号の長さが2ではない");
                System.Diagnostics.Debug.WriteLine("       -> " + channel);
                return false;
            }

            var data = line[(1 + 5 + 1)..].Trim();  // ":"以降

            // 特殊チャンネル [小節の短縮]
            if (channel == "02")
            {
                double mag = double.Parse(data);
                bms.BarMagnification.Add(measure, mag);
                return true;
            }
         
            // エラーチェック : 通常チャンネルのデータ部確認
            if (data.Length % 2 != 0)               // 2桁ごとに区切られる
            {
                System.Diagnostics.Debug.WriteLine("[Warning] ノート定義データ部が偶数桁になっていない");
                System.Diagnostics.Debug.WriteLine("       -> " + data);
                return false;
            }

            // BGMチャンネルの時、サブ(重複)チャンネル処理
            if (channel == "01")
            {
                if (bms.beforeMeasure != measure)
                {
                    bms.beforeMeasure = measure;
                    bms.subChNum = 0;
                }
                else
                {
                    bms.subChNum++;
                }
            }

            var denominator = data.Length / 2;
            for (int i = 0; i < denominator; i++)
            {
                var value = data.Substring(i * 2, 2);

                // 00のデータは無視
                if (value == "00") continue;

                bms.Notes.Add(new Note
                {
                    Channel = channel + ((channel == "01") && (bms.subChNum != 0) ? bms.subChNum.ToString("X02") : string.Empty),
                    Measure = measure,
                    Value = value,
                    Numerator = i,
                    Denominator = denominator
                });
            }

            return true;
        }
    }
}
