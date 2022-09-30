namespace WebTodoAppv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class DetailPageViewModel : BindableBase, IDialogAware
    {
        private readonly TodoDbContext todoDbContext;
        private string commentText;
        private TimeSpan totalWorkingTimeSpan;
        private bool canResetTodo;

        public DetailPageViewModel(TodoDbContext todoDbContext, TodoLists todoLists)
        {
            this.todoDbContext = todoDbContext;
            TodoLists = todoLists;
            Todo = TodoLists.SelectionItem;
            Reload();
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Detail Page";

        public TodoLists TodoLists { get; set; }

        public Todo Todo { get; set; }

        public string CommentText { get => commentText; set => SetProperty(ref commentText, value); }

        public TimeSpan TotalWorkingTimeSpan { get => totalWorkingTimeSpan; private set => SetProperty(ref totalWorkingTimeSpan, value); }

        public bool CanResetTodo { get => canResetTodo; set => SetProperty(ref canResetTodo, value); }

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
            Todo.WorkingState = WorkingState.Completed;
            Reload();
        });

        public DelegateCommand SwitchToIncompleteCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.SwitchToIncomplete, DateTime = DateTime.Now, TodoId = Todo.Id });
            Todo.WorkingState = WorkingState.InitialState;
            Reload();
        });

        public DelegateCommand AddCommentCommand => new DelegateCommand(() =>
        {
            if (Todo != null)
            {
                todoDbContext.AddComment(new Comment()
                {
                    Text = CommentText,
                    TodoId = TodoLists.SelectionItem.Id,
                    DateTime = DateTime.Now,
                });

                Reload();
                CommentText = string.Empty;
            }
        });

        private void Reload()
        {
            if (Todo != null)
            {
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(Todo));
                TotalWorkingTimeSpan = GetTotalWorkingTimeSpan();

                var lastOperation = todoDbContext.Operations
                    .Where(o => o.TodoId == Todo.Id)
                    .OrderBy(o => o.DateTime)
                    .LastOrDefault();

                CanResetTodo = lastOperation is { Kind: OperationKind.Complete };
            }
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            todoDbContext.SaveChanges();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (TodoLists.SelectionItem != null)
            {
                TodoLists.Operations = new ObservableCollection<ITimeTableItem>(todoDbContext.GetOperations(Todo));
            }
        }

        private TimeSpan GetTotalWorkingTimeSpan()
        {
            var operations = todoDbContext.Operations.Where(op => op.TodoId == Todo.Id).OrderBy(op => op.DateTime);

            // はじめに、まだ作業に取り掛かっていない場合と、作業開始せずに完了しているケースを潰す
            if (!operations.Any() || operations.All(op => op.Kind == OperationKind.Complete))
            {
                return TotalWorkingTimeSpan = TimeSpan.Zero;
            }

            // 作業時間算出に必要な変数
            DateTime startPoint = default;
            DateTime endPoint = default;
            TimeSpan total = default;

            foreach (var op in operations)
            {
                if (op.Kind == OperationKind.Start || op.Kind == OperationKind.Resume)
                {
                    startPoint = op.DateTime;
                }

                if (op.Kind == OperationKind.Pause || op.Kind == OperationKind.Complete)
                {
                    endPoint = op.DateTime;
                }

                // 開始点と終了点の組が確定した時点で作業時間を算出して変数を初期化
                if (startPoint != default && endPoint != default)
                {
                    total += endPoint - startPoint;
                    startPoint = default;
                    endPoint = default;
                }
            }

            // 作業を開始後、まだ終了していないケースでは、現在の時間との差を取る
            if (startPoint != default && endPoint == default)
            {
                total += DateTime.Now - startPoint;
            }

            return total;
        }
    }
}