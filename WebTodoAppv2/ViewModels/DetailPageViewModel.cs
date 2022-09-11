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
            Todo = TodoLists.SelectionItem;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Detail Page";

        public TodoLists TodoLists { get; set; }

        public Todo Todo { get; set; }

        public DelegateCommand ChangeTodoStateCommand => new DelegateCommand(() =>
        {
            if (Todo == null)
            {
                return;
            }

            OperationKind operationKind = Todo.WorkingState switch
            {
                WorkingState.InitialState => OperationKind.Start,
                WorkingState.Working => OperationKind.Pause,
                WorkingState.Pausing => OperationKind.Resume,
                _ => throw new InvalidOperationException(),
            };

            todoDbContext.AddOperation(new Operation() { Kind = operationKind, DateTime = DateTime.Now, TodoId = Todo.Id });

            Todo.WorkingState = Todo.WorkingState switch
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
            if (Todo == null)
            {
                return;
            }

            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = Todo.Id });
            Reload();
        });

        public void Reload()
        {
            if (Todo != null)
            {
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(Todo));
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
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(Todo));
            }
        }
    }
}
