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

        public ConnectionPageViewModel()
        {
            // this.dbContext = dbContext;

            // DatabaseName = Properties.Settings.Default.DatabaseName;
            // Port = Properties.Settings.Default.PortNumber;
            // Host = Properties.Settings.Default.Host;
            // Password = Properties.Settings.Default.Password;
            // UserName = Properties.Settings.Default.UserName;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "Connection Page";

        public int Port { get => port; set => SetProperty(ref port, value); }

        public string DatabaseName { get => databaseName; set => SetProperty(ref databaseName, value); }

        public string Host { get => host; set => SetProperty(ref host, value); }

        public string Password { get => password; set => SetProperty(ref password, value); }

        public string UserName { get => userName; set => SetProperty(ref userName, value); }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            // Properties.Settings.Default.DatabaseName = DatabaseName;
            // Properties.Settings.Default.PortNumber = Port;
            // Properties.Settings.Default.Host = Host;
            // Properties.Settings.Default.Password = Password;
            // Properties.Settings.Default.UserName = UserName;
            // Properties.Settings.Default.Save();

            NpgsqlConnectionStringBuilder connectionStringBuilder = new ()
            {
                Database = DatabaseName,
                Port = Port,
                Host = Host,
                Password = Password,
                Username = UserName,
            };

            // dbContext.ConnectionStringBuilder = connectionStringBuilder;
            RequestClose?.Invoke(default);
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