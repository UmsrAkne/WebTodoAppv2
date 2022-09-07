namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prism.Mvvm;

    public class Todo : BindableBase
    {
        private WorkingState workingState;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDateTime { get; set; }

        [NotMapped]
        public WorkingState WorkingState { get => workingState; set => SetProperty(ref workingState, value); }
    }
}
