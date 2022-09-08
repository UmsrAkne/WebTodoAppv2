namespace WebTodoAppv2.Models
{
    using System.Collections.ObjectModel;
    using Prism.Mvvm;

    public class TodoLists : BindableBase
    {
        private ObservableCollection<Todo> todos;
        private ObservableCollection<Operation> operations;
        private Todo selectionItem;

        public ObservableCollection<Todo> Todos { get => todos; set => SetProperty(ref todos, value); }

        public ObservableCollection<Operation> Operations { get => operations; set => SetProperty(ref operations, value); }

        public Todo SelectionItem { get => selectionItem; set => SetProperty(ref selectionItem, value); }
    }
}
