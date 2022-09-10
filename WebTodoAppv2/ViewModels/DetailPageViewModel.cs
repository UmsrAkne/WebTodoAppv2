namespace WebTodoAppv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class DetailPageViewModel : IDialogAware
    {
        private TodoDbContext todoDbContext;

        public DetailPageViewModel(TodoDbContext todoDbContext, TodoLists todoLists)
        {
            this.todoDbContext = todoDbContext;
            TodoLists = todoLists;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Detail Page";

        public TodoLists TodoLists { get; set; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (TodoLists.SelectionItem != null)
            {
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(TodoLists.SelectionItem));
            }
        }
    }
}
