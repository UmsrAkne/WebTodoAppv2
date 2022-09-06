namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Operation
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public OperationKind Kind { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int TodoId { get; set; }
    }
}
