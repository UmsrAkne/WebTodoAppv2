using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using WebTodoAppv2.Models.DBs;

namespace WebTodoAppv2.Models
{
    public class TodoLists : BindableBase
    {
        private ObservableCollection<Todo> todos;
        private ObservableCollection<ITimeTableItem> operations;
        private ObservableCollection<Group> groups;
        private Todo selectionItem;
        private Group currentGroup;

        public ObservableCollection<Todo> Todos { get => todos; set => SetProperty(ref todos, value); }

        public ObservableCollection<ITimeTableItem> Operations { get => operations; set => SetProperty(ref operations, value); }

        public Todo SelectionItem { get => selectionItem; set => SetProperty(ref selectionItem, value); }

        public ObservableCollection<Group> Groups { get => groups; set => SetProperty(ref groups, value); }

        public Group CurrentGroup
        {
            get => currentGroup;
            set
            {
                if (value != null && value != currentGroup)
                {
                    var context = TodoDbContext;
                    Todos = context.Database.CanConnect()
                        ? new ObservableCollection<Todo>(context.GetTodos(value))
                        : new ObservableCollection<Todo>();
                }

                SetProperty(ref currentGroup, value);
            }
        }

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

                return context;
            }
        }
    }
}