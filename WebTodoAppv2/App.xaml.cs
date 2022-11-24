using System.IO;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Unity;
using WebTodoAppv2.Models;
using WebTodoAppv2.Models.DBs;
using WebTodoAppv2.ViewModels;
using WebTodoAppv2.Views;

namespace WebTodoAppv2
{
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
            containerRegistry.RegisterDialog<DetailPage, DetailPageViewModel>();
            containerRegistry.RegisterDialog<TodoAdditionPage, TodoAdditionPageViewModel>();

            IUnityContainer container = containerRegistry.GetContainer();

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

            dbContext.AddDefaultGroup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            const string templateDirectoryName = "templates";
            if (!Directory.Exists(templateDirectoryName))
            {
                Directory.CreateDirectory(templateDirectoryName);

                using var jsonFile = File.CreateText($@"{templateDirectoryName}/template.json");

                jsonFile.WriteLine(@"[");
                jsonFile.WriteLine(@"   {");
                jsonFile.WriteLine(@"       ""Title"" : ""default"",");
                jsonFile.WriteLine(@"       ""Detail"" : """",");
                jsonFile.WriteLine(@"       ""LimitDateTime""  : ""1d"",");
                jsonFile.WriteLine(@"       ""GroupName"" : ""defaultGroupName""");
                jsonFile.WriteLine(@"    }");
                jsonFile.WriteLine(@"]");
            }

            base.OnStartup(e);
        }
    }
}