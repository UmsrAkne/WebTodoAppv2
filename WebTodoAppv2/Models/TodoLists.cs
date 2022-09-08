namespace WebTodoAppv2.Models
{
    using System.Collections.ObjectModel;
    using Prism.Mvvm;

    public class TodoLists : BindableBase
    {
        private ObservableCollection<Todo> todos;
        private Todo selectionItem;

        public ObservableCollection<Todo> Todos { get => todos; set => SetProperty(ref todos, value); }

        public Todo SelectionItem { get => selectionItem; set => SetProperty(ref selectionItem, value); }
    }
}
