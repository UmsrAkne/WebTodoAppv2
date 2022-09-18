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
    using WebTodoAppv2.Views;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web todo app v2";
        private IDialogService dialogService;

        private TodoDbContext todoDbContext;

        public MainWindowViewModel(TodoDbContext dbContext, TodoLists todoLists, IDialogService dialogService)
        {
            todoDbContext = dbContext;
            TodoLists = todoLists;
            TodoLists.CurrentGroup = todoDbContext.GetGroups().FirstOrDefault();
            this.dialogService = dialogService;

            ReloadCommand.Execute();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public TodoLists TodoLists { get; private set; }

        public DelegateCommand ReloadCommand => new DelegateCommand(() =>
        {
            TodoLists.Todos = new ObservableCollection<Todo>(todoDbContext.GetTodos(TodoLists.CurrentGroup));
            TodoLists.Groups = new ObservableCollection<Group>(todoDbContext.GetGroups());
        });

        public DelegateCommand<Todo> CompleteTodoCommand => new DelegateCommand<Todo>((todo) =>
        {
            todoDbContext.AddOperation(new Operation() { Kind = OperationKind.Complete, DateTime = DateTime.Now, TodoId = todo.Id });
            ReloadCommand.Execute();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddGroup(new Group() { Name = "New Group" });
            var currentGroup = TodoLists.CurrentGroup;
            ReloadCommand.Execute();

            TodoLists.CurrentGroup = currentGroup;
        });

        public DelegateCommand<Group> StartGroupNameEditCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = true;
        });

        public DelegateCommand ShowDetailPageCommand => new DelegateCommand(() =>
        {
            if (TodoLists.SelectionItem != null)
            {
                dialogService.ShowDialog(nameof(DetailPage), new DialogParameters(), result => { });
            }
        });

        public DelegateCommand ShowTodoAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(TodoAdditionPage), new DialogParameters(), result => { });
            ReloadCommand.Execute();
        });
    }
}
