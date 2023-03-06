using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace WebTodoAppv2.ViewModels
{
    public class InputDialogViewModel : BindableBase, IDialogAware
    {
        private string message;
        private string text;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand OkCommand => new DelegateCommand(() =>
        {
            var result = new DialogResult(ButtonResult.OK);
            result.Parameters.Add(nameof(Text), Text);
            RequestClose?.Invoke(result);
        });

        public string Message { get => message; private set => SetProperty(ref message, value); }

        public string Text { get => text; set => SetProperty(ref text, value); }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>(nameof(Message));
            Text = parameters.GetValue<string>(nameof(Text));
        }
    }
}