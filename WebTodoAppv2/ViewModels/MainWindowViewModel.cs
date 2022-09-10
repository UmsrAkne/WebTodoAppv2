namespace WebTodoAppv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;
    using WebTodoAppv2.Views;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web todo app v2";
        private IDialogService dialogService;
        private string inputText;
        private string commentText;

        private TodoDbContext todoDbContext;

        public MainWindowViewModel(TodoDbContext dbContext, TodoLists todoLists, IDialogService dialogService)
        {
            todoDbContext = dbContext;
            TodoLists = todoLists;
            this.dialogService = dialogService;

            ReloadCommand.Execute();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public string CommentText { get => commentText; set => SetProperty(ref commentText, value); }

        public TodoLists TodoLists { get; private set; }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddTodo(new Todo { Title = InputText, CreationDateTime = DateTime.Now });
            InputText = string.Empty;
            ReloadCommand.Execute();
        });

        public DelegateCommand AddCommentCommand => new DelegateCommand(() =>
        {
            if (TodoLists.SelectionItem != null)
            {
                todoDbContext.AddComment(new Comment()
                {
                    Text = CommentText,
                    TodoId = TodoLists.SelectionItem.Id,
                    DateTime = DateTime.Now,
                });

                ReloadCommand.Execute();
                CommentText = string.Empty;
            }
        });

        public DelegateCommand ReloadCommand => new DelegateCommand(() =>
        {
            TodoLists.Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos());

            if (TodoLists.SelectionItem != null)
            {
                ShowTodoDetailCommand.Execute(TodoLists.SelectionItem);
            }
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
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(todo));
            }
        });

        public DelegateCommand ShowDetailPageCommand => new DelegateCommand(() =>
        {
            if (TodoLists.SelectionItem != null)
            {
                dialogService.ShowDialog(nameof(DetailPage), new DialogParameters(), result => { });
            }
        });
    }
}
