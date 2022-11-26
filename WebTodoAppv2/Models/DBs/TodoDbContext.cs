using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebTodoAppv2.Models.DBs
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Operation> Operations { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Todo> Todos { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Comment> Comments { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Group> Groups { get; set; }

        public void AddTodo(Todo todo)
        {
            if (todo.GroupName != string.Empty)
            {
                var group = Groups.FirstOrDefault(g => g.Name == todo.GroupName);
                todo.GroupId = group?.Id ?? 1;
            }

            Todos.Add(todo);
            SaveChanges();
        }

        public void AddOperation(Operation operation)
        {
            Operations.Add(operation);
            SaveChanges();
        }

        public void AddComment(Comment comment)
        {
            Comments.Add(comment);
            SaveChanges();
        }

        /// <summary>
        /// グループのテーブルにレコードがなかった場合のみ、デフォルトのグループを追加します
        /// アプリ起動時、 App クラスで DIコンテナに型を登録するタイミングで呼び出されます。
        /// </summary>
        public void AddDefaultGroup()
        {
            if (!Database.CanConnect())
            {
                return;
            }

            if (!Groups.Any())
            {
                Groups.Add(new Group() { Name = "Default Group" });
                SaveChanges();
            }
        }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
            SaveChanges();
        }

        public List<Todo> GetTodos(Group group)
        {
            return GetTodos().Where(t => group.Id == t.GroupId).ToList();
        }

        /// <summary>
        /// 入力された todo に対して行った操作一覧を詰めたリストを取得します。
        /// </summary>
        /// <param name="todo">操作ログを取得する Todo</param>
        /// <returns>todo に対して行った操作一覧を詰めたリスト</returns>
        public List<ITimeTableItem> GetOperations(Todo todo)
        {
            var comments = Comments.Where(c => todo.Id == c.TodoId).Cast<ITimeTableItem>();
            var operations = Operations.Where(op => todo.Id == op.TodoId).Cast<ITimeTableItem>();
            return new List<ITimeTableItem>() { todo }
                .Concat(comments)
                .Concat(operations)
                .OrderBy(ti => ti.DateTime).ToList();
        }

        public List<Group> GetGroups()
        {
            return Groups.Where(g => true).ToList();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Port = Properties.Settings.Default.PortNumber,
                Username = Properties.Settings.Default.UserName,
                Password = Properties.Settings.Default.Password,
                Host = Properties.Settings.Default.Host,
                Database = Properties.Settings.Default.DatabaseName,
            };

            optionsBuilder.UseNpgsql(builder.ToString());
        }

        private List<Todo> GetTodos()
        {
            var todos = Todos.Where(t => true).OrderBy(t => t.Id).ToList();

            var joinedTodos = todos.Join(
                Operations,
                t => t.Id,
                op => op.TodoId,
                (t, op) => new { todo = t, operation = op });

            var todoGroups = joinedTodos.GroupBy(t => t.todo.Id).ToList();

            foreach (var tg in todoGroups)
            {
                if (tg == null)
                {
                    continue;
                }

                var lastOperationKind = tg.MaxBy(t => t.operation.DateTime) !.operation.Kind;
                var currentTodo = tg.FirstOrDefault()?.todo;

                if (currentTodo == null)
                {
                    continue;
                }

                switch (lastOperationKind)
                {
                    case OperationKind.Complete:
                        currentTodo.WorkingState = WorkingState.Completed;
                        continue;
                    case OperationKind.SwitchToIncomplete:
                        continue;
                    case OperationKind.Pause:
                        currentTodo.WorkingState = WorkingState.Pausing;
                        continue;
                    case OperationKind.Start:
                    case OperationKind.Resume:
                        currentTodo.WorkingState = WorkingState.Working;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return todos;
        }
    }
}