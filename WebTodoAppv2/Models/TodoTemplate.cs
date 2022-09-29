using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WebTodoAppv2.Models
{
    public class TodoTemplate
    {
        public string Title { get; set; }

        public string Detail { get; set; }

        public string GroupName { get; set; }

        [JsonPropertyName("LimitDateTime")]
        public string LimitDateTimeString { get; set; }

        public TimeSpan LimitTime
        {
            get
            {
                if (!Regex.IsMatch(LimitDateTimeString, "\\d+[mhd]"))
                {
                    return TimeSpan.FromDays(1);
                }

                var m = Regex.Match(LimitDateTimeString, "(\\d+)([mhd])");

                return m.Groups[2].Value switch
                {
                    "m" => TimeSpan.FromMinutes(int.Parse(m.Groups[1].Value)),
                    "h" => TimeSpan.FromHours(int.Parse(m.Groups[1].Value)),
                    "d" => TimeSpan.FromDays(int.Parse(m.Groups[1].Value)),
                    _ => TimeSpan.FromDays(1)
                };
            }
        }
    }
}