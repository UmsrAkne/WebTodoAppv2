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
        private TodoDbContext todoDbContext = new ();
        private bool databaseConnection;
        private string title = "Web todo app v2";

        private int completeTodoCount;

        public MainWindowViewModel(IDialogService dialogService)
        {
            try
            {
                todoDbContext.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            DatabaseConnection = todoDbContext.Database.CanConnect();

            if (DatabaseConnection)
            {
                TopTodoLists.CurrentGroup = todoDbContext.GetGroups().FirstOrDefault();
                BottomTodoLists.CurrentGroup = todoDbContext.GetGroups().FirstOrDefault();
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
            TopTodoLists.Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos(TopTodoLists.CurrentGroup));
            TopTodoLists.Groups = new ObservableCollection<Group>(todoDbContext.GetGroups());

            BottomTodoLists.Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos(BottomTodoLists.CurrentGroup));
            BottomTodoLists.Groups = new ObservableCollection<Group>(todoDbContext.GetGroups());

            CompleteTodoCount = TopTodoLists.Todos.Count(t => t.WorkingState == WorkingState.Completed);
        });

        public DelegateCommand<Todo> CompleteTodoCommand => new DelegateCommand<Todo>((todo) =>
        {
            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = todo.Id });
            ReloadCommand.Execute();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddGroup(new Group() { Name = "New Group" });
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
            todoDbContext.SaveChanges();
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

               todoDbContext.AddTodo(todo);
           });

           ReloadCommand.Execute();
        }
    }
}