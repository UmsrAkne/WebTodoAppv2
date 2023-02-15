using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace WebTodoAppv2.Models
{
    public class Todo : BindableBase, ITimeTableItem
    {
        private WorkingState workingState;
        private string title = string.Empty;
        private string detail = string.Empty;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
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

        [Required]
        public long DurationTicks { get; set; }

        [NotMapped]
        public WorkingState WorkingState { get => workingState; set => SetProperty(ref workingState, value); }

        [NotMapped]
        public DateTime DateTime { get => CreationDateTime; set => CreationDateTime = value; }

        [NotMapped]
        public string Text { get => "Created"; set => _ = value; }

        [NotMapped]
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        /// 入力した Operation.Kind に合わせて、WorkingState を更新します。
        /// </summary>
        /// <param name="operation">適用する Operation を入力します。Operation.Kind のみが使用されます。</param>
        /// <exception cref="ArgumentOutOfRangeException">operation.Kind が
        /// Start, Pause, Resume, Complete, SwitchToIncomplete 以外だった場合にスロー</exception>
        public void ApplyOperation(Operation operation)
        {
            WorkingState = operation.Kind switch
            {
                OperationKind.Start => WorkingState.Working,
                OperationKind.Pause => WorkingState.Pausing,
                OperationKind.Resume => WorkingState.Working,
                OperationKind.Complete => WorkingState.Completed,
                OperationKind.SwitchToIncomplete => WorkingState.InitialState,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}