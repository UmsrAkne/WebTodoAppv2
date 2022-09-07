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
