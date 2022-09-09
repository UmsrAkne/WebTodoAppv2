namespace WebTodoAppv2.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class TodoDbContext : DbContext
    {
        public TodoDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Todo> Todos { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public void AddTodo(Todo todo)
        {
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

        public List<Todo> GetTodos()
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
                var lastOperationKind = tg.OrderBy(t => t.operation.DateTime).LastOrDefault().operation.Kind;
                var currentTodo = tg.FirstOrDefault().todo;

                if (lastOperationKind == OperationKind.Complete)
                {
                    currentTodo.WorkingState = WorkingState.Completed;
                    continue;
                }

                if (lastOperationKind == OperationKind.SwitchToIncomplete)
                {
                    continue;
                }

                if (lastOperationKind == OperationKind.Pause)
                {
                    currentTodo.WorkingState = WorkingState.Pausing;
                    continue;
                }

                if (lastOperationKind == OperationKind.Start || lastOperationKind == OperationKind.Resume)
                {
                    currentTodo.WorkingState = WorkingState.Working;
                }
            }

            return todos;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Port = 5433;
            builder.Username = "postgres";
            builder.Password = "password";
            builder.Host = "localhost";
            builder.Database = "testdb";

            optionsBuilder.UseNpgsql(builder.ToString());
        }
    }
}
