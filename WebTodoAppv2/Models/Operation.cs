namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Operation : ITimeTableItem
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

        [NotMapped]
        public string Text { get => Kind.ToString(); set => _ = value; }
    }
}
