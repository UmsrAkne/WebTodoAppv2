using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using WebTodoAppv2.Models;
using WebTodoAppv2.Models.DBs;
using WebTodoAppv2.Views;

namespace WebTodoAppv2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private bool databaseConnection;
        private string title = "Web todo app v2";
        private string limitDateTimeText = "0d";
        private string durationTicksText = "60m";
        private bool isMultiLineView;

        private int completeTodoCount;
        private Todo currentTodo = new Todo();

        private DelegateCommand addTodoCommand;
        private DelegateCommand copyTodoCommand;

        public MainWindowViewModel(IDialogService dialogService)
        {
            using var context = TodoDbContext;

            try
            {
                context.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            DatabaseConnection = context.Database.CanConnect();

            if (DatabaseConnection)
            {
                TopTodoLists.CurrentGroup ??= context.GetGroups().FirstOrDefault();
                Reload();
            }

            this.dialogService = dialogService;
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public bool DatabaseConnection { get => databaseConnection; private set => SetProperty(ref databaseConnection, value); }

        public TodoLists TopTodoLists { get; } = new ();

        public Todo CurrentTodo { get => currentTodo; private set => SetProperty(ref currentTodo, value); }

        public bool IsMultiLineView { get => isMultiLineView; set => SetProperty(ref isMultiLineView, value); }

        // ReSharper disable once MemberCanBePrivate.Global
        public int CompleteTodoCount { get => completeTodoCount; set => SetProperty(ref completeTodoCount, value); }

        public string LimitDateTimeText { get => limitDateTimeText; set => SetProperty(ref limitDateTimeText, value); }

        public string DurationTicksText { get => durationTicksText; set => SetProperty(ref durationTicksText, value); }

        public DelegateCommand<Todo> CompleteTodoCommand => new DelegateCommand<Todo>((todo) =>
        {
            using var context = TodoDbContext;
            context.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = todo.Id });
            Reload();
        });

        public DelegateCommand AddTodoCommand => addTodoCommand ??= new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(CurrentTodo.Title + CurrentTodo.Detail))
            {
                return;
            }

            var todo = new Todo()
            {
                Title = CurrentTodo.Title,
                Detail = CurrentTodo.Detail,
                CreationDateTime = DateTime.Now,
                GroupName = TopTodoLists.CurrentGroup.Name,
                LimitDateTime = DateTimeTextConverter.ConvertDateTimeText(LimitDateTimeText, DateTime.Now),
                DurationTicks = DateTimeTextConverter.ConvertTimeSpanText(DurationTicksText).Ticks,
            };

            if (todo.LimitDateTime == default)
            {
                todo.LimitDateTime = DateTimeTextConverter.ConvertDateTimeText("0d", DateTime.Now);
            }

            using var context = TodoDbContext;
            context.AddTodo(todo);
            CurrentTodo = new Todo();
            Reload();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            using var context = TodoDbContext;
            context.AddGroup(new Group() { Name = "New Group" });
            var currentGroup = TopTodoLists.CurrentGroup;
            Reload();

            TopTodoLists.CurrentGroup = currentGroup;
        });

        public DelegateCommand<Group> StartGroupNameEditCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = true;
        });

        public DelegateCommand<Group> ConfirmGroupNameCommand => new DelegateCommand<Group>((group) =>
        {
            using var context = TodoDbContext;
            context.SaveChanges();
            group.EditMode = false;
        });

        public DelegateCommand<Todo> ShowDetailPageCommand => new DelegateCommand<Todo>((t) =>
        {
            dialogService.ShowDialog(nameof(DetailPage), new DialogParameters() { { nameof(Todo), t } }, _ => { });
            Reload();
        });

        public DelegateCommand<Group> ShowTodoAdditionPageCommand => new DelegateCommand<Group>((group) =>
        {
            dialogService.ShowDialog(nameof(TodoAdditionPage), new DialogParameters() { { nameof(Group), group } }, _ => { });
            Reload();
        });

        public DelegateCommand ShowConnectionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(ConnectionPage), new DialogParameters(), _ => { });

            using var context = TodoDbContext;
            TopTodoLists.CurrentGroup ??= context.GetGroups().FirstOrDefault();

            Reload();
        });

        public DelegateCommand<Todo> ChangeTodoStatusCommand => new DelegateCommand<Todo>(t =>
        {
            var operation = t.WorkingState switch
                {
                    WorkingState.InitialState => new Operation() { Kind = OperationKind.Start },
                    WorkingState.Pausing => new Operation() { Kind = OperationKind.Resume },
                    WorkingState.Working => new Operation() { Kind = OperationKind.Pause },
                    WorkingState.Completed => null,
                    _ => null
                };

            if (operation == null)
            {
                return;
            }

            operation.TodoId = t.Id;
            operation.DateTime = DateTime.Now;
            t.ApplyOperation(operation);

            using var context = TodoDbContext;
            context.AddOperation(operation);
        });

        public DelegateCommand CopyTodoCommand => copyTodoCommand ??= new DelegateCommand(() =>
        {
            if (TopTodoLists.SelectionItem == null)
            {
                return;
            }

            CurrentTodo = TopTodoLists.SelectionItem.GetCopy();
        });

        public DelegateCommand ToggleViewModeCommand => new DelegateCommand(() => IsMultiLineView = !IsMultiLineView);

        private TodoDbContext TodoDbContext
        {
            get
            {
                var context = new TodoDbContext();
                try
                {
                    context.Database.EnsureCreated();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                DatabaseConnection = context.Database.CanConnect();

                return context;
            }
        }

        public void AddTodo(List<TodoTemplate> templates)
        {
           templates.ForEach(t =>
           {
               var todo = new Todo
               {
                   Title = t.Title,
                   Detail = t.Detail,
                   LimitDateTime = DateTime.Now + t.LimitTime,
                   CreationDateTime = DateTime.Now,
                   GroupName = t.GroupName,
               };

               using var context = TodoDbContext;
               context.AddTodo(todo);
           });

           Reload();
        }

        private void Reload()
        {
            TopTodoLists.Reload();
            CompleteTodoCount = TopTodoLists.Todos.Count(t => t.WorkingState == WorkingState.Completed);
        }
    }
}