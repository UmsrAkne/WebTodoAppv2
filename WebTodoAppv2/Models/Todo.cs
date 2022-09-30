namespace WebTodoAppv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prism.Mvvm;

    public class Todo : BindableBase, ITimeTableItem
    {
        private WorkingState workingState;
        private string title = string.Empty;
        private string detail = string.Empty;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get => title; set => SetProperty(ref title, value); }

        [Required]
        public string Detail { get => detail; set => SetProperty(ref detail, value); }

        [Required]
        public DateTime CreationDateTime { get; set; }

        [Required]
        public DateTime LimitDateTime { get; set; }

        [Required]
        public int GroupId { get; set; }

        [NotMapped]
        public WorkingState WorkingState { get => workingState; set => SetProperty(ref workingState, value); }

        [NotMapped]
        public DateTime DateTime { get => CreationDateTime; set => CreationDateTime = value; }

        [NotMapped]
        public string Text { get => "Created"; set => _ = value; }

        [NotMapped]
        public string GroupName { get; set; } = string.Empty;
    }
}