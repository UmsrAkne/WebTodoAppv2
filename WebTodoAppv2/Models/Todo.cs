namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Todo
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}
