using System.IO;
using System.Windows;
using Prism.Ioc;
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
            containerRegistry.RegisterDialog<ConnectionPage, ConnectionPageViewModel>();

            var dbContext = new TodoDbContext();

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