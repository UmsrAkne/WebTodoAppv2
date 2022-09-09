namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Comment : ITimeTableItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int TodoId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
    }
}
