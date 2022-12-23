using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int completeTodoCount;

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
                BottomTodoLists.CurrentGroup ??= context.GetGroups().FirstOrDefault();
                ReloadCommand.Execute();
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

        public TodoLists BottomTodoLists { get; } = new ();

        // ReSharper disable once MemberCanBePrivate.Global
        public int CompleteTodoCount { get => completeTodoCount; set => SetProperty(ref completeTodoCount, value); }

        public DelegateCommand ReloadCommand => new DelegateCommand(() =>
        {
            using var context = TodoDbContext;

            if (!DatabaseConnection)
            {
                return;
            }

            TopTodoLists.Todos = new ObservableCollection<Todo>(context.GetTodos(TopTodoLists.CurrentGroup));
            TopTodoLists.Groups = new ObservableCollection<Group>(context.GetGroups());

            BottomTodoLists.Todos = new ObservableCollection<Todo>(context.GetTodos(BottomTodoLists.CurrentGroup));
            BottomTodoLists.Groups = new ObservableCollection<Group>(context.GetGroups());

            CompleteTodoCount = TopTodoLists.Todos.Count(t => t.WorkingState == WorkingState.Completed);
        });

        public DelegateCommand<Todo> CompleteTodoCommand => new DelegateCommand<Todo>((todo) =>
        {
            using var context = TodoDbContext;
            context.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = todo.Id });
            ReloadCommand.Execute();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            using var context = TodoDbContext;
            context.AddGroup(new Group() { Name = "New Group" });
            var currentGroup = TopTodoLists.CurrentGroup;
            ReloadCommand.Execute();

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
            ReloadCommand.Execute();
        });

        public DelegateCommand<Group> ShowTodoAdditionPageCommand => new DelegateCommand<Group>((group) =>
        {
            dialogService.ShowDialog(nameof(TodoAdditionPage), new DialogParameters() { { nameof(Group), group } }, _ => { });
            ReloadCommand.Execute();
        });

        public DelegateCommand ShowConnectionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(ConnectionPage), new DialogParameters(), _ => { });

            using var context = TodoDbContext;
            TopTodoLists.CurrentGroup ??= context.GetGroups().FirstOrDefault();
            BottomTodoLists.CurrentGroup ??= context.GetGroups().FirstOrDefault();

            ReloadCommand.Execute();
        });

        public DelegateCommand<Todo> ChangeTodoStatusCommand => new DelegateCommand<Todo>(t =>
        {
            if (t.WorkingState == WorkingState.Completed)
            {
                return;
            }

            var operation = t.WorkingState switch
                {
                    WorkingState.InitialState => new Operation() { Kind = OperationKind.Start },
                    WorkingState.Pausing => new Operation() { Kind = OperationKind.Resume },
                    WorkingState.Working => new Operation() { Kind = OperationKind.Pause },
                    _ => throw new ArgumentOutOfRangeException()
                };
            // ArgumentException とかを投げるとプロパティで発生させることができない例外との指摘がでる
            // プロパティで発生させられない例外ってことだけど、例外は投げるなってこと？

            operation.TodoId = t.Id;
            operation.DateTime = DateTime.Now;
            
            using var context = TodoDbContext;
            context.AddOperation(operation);
        });


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

           ReloadCommand.Execute();
        }
    }
}