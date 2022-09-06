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
            return Todos.Where(t => true).OrderBy(t => t.Id).ToList();
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
