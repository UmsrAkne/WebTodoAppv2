using System;
using System.Text.RegularExpressions;

namespace WebTodoAppv2.Models
{
    public class DateTimeTextConverter
    {
        /// <summary>
        /// 特定の書式のテキストを DateTime に変換します。
        /// </summary>
        /// <param name="text">"0d", "1h", "10m", "30s" と言った書式に則ったテキストを入力します。</param>
        /// <param name="nowDateTime">現在の時間を表す DateTime を入力します。</param>
        /// <returns>nowDateTime から text の時間分ずれた DateTime を返します。
        /// ただし、text が指定書式通りでなかった場合、DateTime の既定値を返します。
        /// </returns>
        public static DateTime ConvertDateTimeText(string text, DateTime nowDateTime)
        {
            text = text.Replace(" ", string.Empty); // 予め空白だけは削除しておく

            if (string.IsNullOrWhiteSpace(text) || !Regex.IsMatch(text, "^\\d+[dhms]$"))
            {
                // 所定フォーマットに合致しない場合は既定値を返す
                return default;
            }

            var match = Regex.Match(text, "^(\\d+)([dhms])$");
            var amount = int.Parse(match.Groups[1].Value);
            var unit = match.Groups[2].Value;

            return unit switch
            {
                "d" => nowDateTime.Date + TimeSpan.FromDays(amount),
                "h" => nowDateTime + TimeSpan.FromHours(amount),
                "m" => nowDateTime + TimeSpan.FromMinutes(amount),
                "s" => nowDateTime + TimeSpan.FromSeconds(amount),
                _ => default,
            };
        }
    }
}