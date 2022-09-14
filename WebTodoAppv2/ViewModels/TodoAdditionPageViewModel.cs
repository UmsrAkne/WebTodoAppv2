namespace WebTodoAppv2.ViewModels
{
    using System;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class TodoAdditionPageViewModel : BindableBase, IDialogAware
    {
        private TodoDbContext todoDbContext;
        private TodoLists todoLists;

        private string todoTitle;

        // デフォルトは ２４時間後を期限とする
        private string remainingHour = TimeSpan.FromDays(1).TotalHours.ToString();

        public TodoAdditionPageViewModel(TodoDbContext todoDbContext, TodoLists todoLists)
        {
            this.todoDbContext = todoDbContext;
            this.todoLists = todoLists;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Create new Todo";

        public string TodoTitle { get => todoTitle; set => SetProperty(ref todoTitle, value); }

        public string RemainingHour { get => remainingHour; set => SetProperty(ref remainingHour, value); }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            var todo = new Todo { Title = TodoTitle, CreationDateTime = DateTime.Now };

            if (int.TryParse(RemainingHour, out var remainingTime))
            {
                todo.LimitDateTime = DateTime.Now.AddHours(remainingTime);
            }
            else
            {
                todo.LimitDateTime = DateTime.Now.AddDays(1);
            }

            todoDbContext.AddTodo(todo);
            RequestClose.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
