namespace WebTodoAppv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Prism.Commands;
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

        public DelegateCommand ChangeTodoStateCommand => new DelegateCommand(() =>
        {
            if (TodoLists.SelectionItem == null)
            {
                return;
            }

            var todo = TodoLists.SelectionItem;

            OperationKind operationKind = todo.WorkingState switch
            {
                WorkingState.InitialState => OperationKind.Start,
                WorkingState.Working => OperationKind.Pause,
                WorkingState.Pausing => OperationKind.Resume,
                _ => throw new InvalidOperationException(),
            };

            todoDbContext.AddOperation(new Operation() { Kind = operationKind, DateTime = DateTime.Now, TodoId = todo.Id });

            todo.WorkingState = todo.WorkingState switch
            {
                WorkingState.InitialState => WorkingState.Working,
                WorkingState.Working => WorkingState.Pausing,
                WorkingState.Pausing => WorkingState.Working,
                _ => throw new InvalidOperationException(),
            };

            Reload();
        });

        public DelegateCommand CompleteTodoCommand => new DelegateCommand(() =>
        {
            if (TodoLists.SelectionItem == null)
            {
                return;
            }

            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = TodoLists.SelectionItem.Id });
            Reload();
        });

        public void Reload()
        {
            if (TodoLists.SelectionItem != null)
            {
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(TodoLists.SelectionItem));
            }
        }

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
