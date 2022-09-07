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
        private ObservableCollection<Todo> todos;

        private TodoDbContext todoDbContext;

        public MainWindowViewModel(TodoDbContext dbContext)
        {
            todoDbContext = dbContext;

            ReloadCommand.Execute();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public ObservableCollection<Todo> Todos { get => todos; set => SetProperty(ref todos, value); }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddTodo(new Todo { Title = InputText, CreationDateTime = DateTime.Now });
            InputText = string.Empty;
            ReloadCommand.Execute();
        });

        public DelegateCommand ReloadCommand => new DelegateCommand(() =>
        {
            Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos());
        });

        public DelegateCommand<Todo> StartTodoCommand => new DelegateCommand<Todo>((param) =>
        {
            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Start, DateTime = DateTime.Now, TodoId = param.Id });
        });
    }
}
