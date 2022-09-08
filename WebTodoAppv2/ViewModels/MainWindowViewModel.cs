namespace WebTodoAppv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Prism.Commands;
    using Prism.Mvvm;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web todo app v2";
        private string inputText;

        private TodoDbContext todoDbContext;

        public MainWindowViewModel(TodoDbContext dbContext, TodoLists todoLists)
        {
            todoDbContext = dbContext;
            TodoLists = todoLists;

            ReloadCommand.Execute();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public TodoLists TodoLists { get; private set; }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddTodo(new Todo { Title = InputText, CreationDateTime = DateTime.Now });
            InputText = string.Empty;
            ReloadCommand.Execute();
        });

        public DelegateCommand ReloadCommand => new DelegateCommand(() =>
        {
            TodoLists.Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos());
        });

        public DelegateCommand<Todo> ChangeTodoStateCommand => new DelegateCommand<Todo>((param) =>
        {
            OperationKind operationKind = param.WorkingState switch
            {
                WorkingState.InitialState => OperationKind.Start,
                WorkingState.Working => OperationKind.Pause,
                WorkingState.Pausing => OperationKind.Resume,
                _ => throw new InvalidOperationException(),
            };

            todoDbContext.AddOperation(new Operation() { Kind = operationKind, DateTime = DateTime.Now, TodoId = param.Id });
            ReloadCommand.Execute();
        });

        public DelegateCommand<Todo> CompleteTodoCommand => new DelegateCommand<Todo>((todo) =>
        {
            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = todo.Id });
            ReloadCommand.Execute();
        });

        public DelegateCommand<Todo> ShowTodoDetailCommand => new DelegateCommand<Todo>((todo) =>
        {
            if (todo != null)
            {
                TodoLists.Operations = new ObservableCollection<Operation>(todoDbContext.GetOperations(todo));
            }
        });
    }
}
