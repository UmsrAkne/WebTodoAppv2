namespace WebTodoAppv2.Models
{
    using System.Collections.ObjectModel;
    using Prism.Mvvm;

    public class TodoLists : BindableBase
    {
        private ObservableCollection<Todo> todos;
        private ObservableCollection<ITimeTableItem> operations;
        private Todo selectionItem;
        private Group currentGroup;

        public ObservableCollection<Todo> Todos { get => todos; set => SetProperty(ref todos, value); }

        public ObservableCollection<ITimeTableItem> Operations { get => operations; set => SetProperty(ref operations, value); }

        public Todo SelectionItem { get => selectionItem; set => SetProperty(ref selectionItem, value); }

        public Group CurrentGroup { get => currentGroup; set => SetProperty(ref currentGroup, value); }
    }
}
