namespace WebTodoAppv2.ViewModels
{
    using Prism.Mvvm;
    using WebTodoAppv2.Models.DBs;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web todo app v2";
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
    }
}
