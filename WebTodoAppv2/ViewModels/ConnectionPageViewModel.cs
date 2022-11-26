using System;
using Npgsql;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using WebTodoAppv2.Models.DBs;

namespace WebTodoAppv2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConnectionPageViewModel : BindableBase, IDialogAware
    {
        private readonly TodoDbContext dbContext;
        private int port;
        private string databaseName;
        private string host;
        private string password;
        private string userName;
        private bool connection;

        public ConnectionPageViewModel()
        {
            DatabaseName = Properties.Settings.Default.DatabaseName;
            Port = Properties.Settings.Default.PortNumber;
            Host = Properties.Settings.Default.Host;
            Password = Properties.Settings.Default.Password;
            UserName = Properties.Settings.Default.UserName;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Connection Page";

        public int Port { get => port; set => SetProperty(ref port, value); }

        public string DatabaseName { get => databaseName; set => SetProperty(ref databaseName, value); }

        public string Host { get => host; set => SetProperty(ref host, value); }

        public string Password { get => password; set => SetProperty(ref password, value); }

        public string UserName { get => userName; set => SetProperty(ref userName, value); }

        public bool Connection { get => connection; set => SetProperty(ref connection, value); }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            Properties.Settings.Default.DatabaseName = DatabaseName;
            Properties.Settings.Default.PortNumber = Port;
            Properties.Settings.Default.Host = Host;
            Properties.Settings.Default.Password = Password;
            Properties.Settings.Default.UserName = UserName;
            Properties.Settings.Default.Save();

            RequestClose?.Invoke(default);
        });

        public DelegateCommand ConnectionCommand => new DelegateCommand(() =>
        {
            var context = new TodoDbContext();

            try
            {
                context.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                // ignored
            }

            Connection = context.Database.CanConnect();
        });

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}