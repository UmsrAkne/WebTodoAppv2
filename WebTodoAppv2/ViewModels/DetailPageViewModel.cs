namespace WebTodoAppv2.ViewModels
{
    using System;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class DetailPageViewModel : IDialogAware
    {
        private TodoDbContext todoDbContext;
        private TodoLists todoLists;

        public DetailPageViewModel(TodoDbContext todoDbContext, TodoLists todoLists)
        {
            this.todoDbContext = todoDbContext;
            this.todoLists = todoLists;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Detail Page";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
