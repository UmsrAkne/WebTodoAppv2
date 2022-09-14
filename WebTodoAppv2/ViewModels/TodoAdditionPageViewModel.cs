namespace WebTodoAppv2.ViewModels
{
    using System;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class TodoAdditionPageViewModel : IDialogAware
    {
        private TodoDbContext todoDbContext;
        private TodoLists todoLists;

        public TodoAdditionPageViewModel(TodoDbContext todoDbContext, TodoLists todoLists)
        {
            this.todoDbContext = todoDbContext;
            this.todoLists = todoLists;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Create new Todo";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
