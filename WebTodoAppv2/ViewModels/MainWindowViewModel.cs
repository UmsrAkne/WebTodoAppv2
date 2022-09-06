namespace WebTodoAppv2.ViewModels
{
    using System;
    using Prism.Commands;
    using Prism.Mvvm;
    using WebTodoAppv2.Models;
    using WebTodoAppv2.Models.DBs;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web todo app v2";
        private string inputText;
        private TodoDbContext todoDbContext;

        public MainWindowViewModel(TodoDbContext dbContext)
        {
            todoDbContext = dbContext;
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public DelegateCommand AddTodoCommand => new DelegateCommand(() =>
        {
            todoDbContext.AddTodo(new Todo { Title = InputText, CreationDateTime = DateTime.Now });
            InputText = string.Empty;
        });
    }
}
