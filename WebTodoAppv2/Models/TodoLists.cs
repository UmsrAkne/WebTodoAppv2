namespace WebTodoAppv2.Models
{
    using System.Collections.ObjectModel;
    using Prism.Mvvm;

    public class TodoLists : BindableBase
    {
        private Todo selectionItem;

        public ObservableCollection<Todo> Todos { get; set; }

        public Todo SelectionItem { get => selectionItem; set => SetProperty(ref selectionItem, value); }
    }
}
