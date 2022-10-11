using System;
using System.ComponentModel.DataAnnotations;

namespace WebTodoAppv2.Models
{
    public class Comment : ITimeTableItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        [Required]
        public int TodoId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
    }
}