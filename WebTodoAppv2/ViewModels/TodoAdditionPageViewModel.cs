using System;
using System.Globalization;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using WebTodoAppv2.Models;
using WebTodoAppv2.Models.DBs;

namespace WebTodoAppv2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TodoAdditionPageViewModel : BindableBase, IDialogAware
    {
        private readonly TodoDbContext todoDbContext;
        private string todoTitle = string.Empty;
        private string detail = string.Empty;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Group currentGroup;

        // デフォルトは ２４時間後を期限とする
        private string remainingHour = TimeSpan.FromDays(1).TotalHours.ToString(CultureInfo.InvariantCulture);

        private bool createAsCompletedTodo;

        public TodoAdditionPageViewModel(TodoDbContext todoDbContext)
        {
            this.todoDbContext = todoDbContext;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Create new Todo";

        public string TodoTitle { get => todoTitle; set => SetProperty(ref todoTitle, value); }

        public string Detail { get => detail; set => SetProperty(ref detail, value); }

        public string RemainingHour { get => remainingHour; set => SetProperty(ref remainingHour, value); }

        public bool CreateAsCompletedTodo { get => createAsCompletedTodo; set => SetProperty(ref createAsCompletedTodo, value); }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            var todo = new Todo
            {
                Title = TodoTitle,
                Detail = Detail,
                CreationDateTime = DateTime.Now,
                GroupId = currentGroup.Id,
                LimitDateTime = int.TryParse(RemainingHour, out var remainingTime)
                    ? DateTime.Now.AddHours(remainingTime)
                    : DateTime.Now.AddDays(1),
            };

            // AddTodo で追加した時点で todo に ID が割り振らている
            todoDbContext.AddTodo(todo);

            if (CreateAsCompletedTodo)
            {
                var op = new Operation()
                {
                    TodoId = todo.Id,
                    DateTime = DateTime.Now,
                    Kind = OperationKind.Complete,
                };

                todoDbContext.AddOperation(op);
            }

            RequestClose?.Invoke(default(DialogResult));
        });

        public DelegateCommand CancelCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(default(DialogResult));
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            currentGroup = parameters.GetValue<Group>(nameof(Group));
        }
    }
}