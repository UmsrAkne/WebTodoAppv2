﻿namespace WebTodoAppv2
{
    using System.Windows;
    using Prism.Ioc;
    using Unity;
    using WebTodoAppv2.Models.DBs;
    using WebTodoAppv2.Views;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // DI する対象が具象クラスである場合は RegisterType の必要はないかも？　(未検証)
            container.RegisterType(typeof(TodoDbContext));

            // 前述の RegisterType を削除しても Singleton に登録は可能。
            container.RegisterSingleton(typeof(TodoDbContext));

            var dbContext = container.Resolve<TodoDbContext>();

            try
            {
                dbContext.Database.EnsureCreated();
            }
            catch (Npgsql.NpgsqlException)
            {
                // SystemMessage = "PostgreSQL データベースへの接続に失敗しました";
            }

            base.OnStartup(e);
        }
    }
}
