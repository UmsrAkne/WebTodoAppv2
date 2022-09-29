using System.Linq;

namespace WebTodoAppv2.Models
{
    using System.IO;
    using System.Text.Json;
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
    using WebTodoAppv2.ViewModels;

    public class DropBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // ファイルをドラッグしてきて、コントロール上に乗せた際の処理
            AssociatedObject.PreviewDragOver += AssociatedObject_PreviewDragOver;

            // ファイルをドロップした際の処理
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDragOver -= AssociatedObject_PreviewDragOver;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            // ファイルパスの一覧の配列
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var vm = ((Window)sender).DataContext as MainWindowViewModel;

            if (files == null)
            {
                return;
            }

            using var sr = new StreamReader(files[0]);
            var jsonText = sr.ReadToEnd();
            var todos = JsonSerializer.Deserialize<TodoTemplate[]>(jsonText, new JsonSerializerOptions());

            if (vm != null && todos != null)
            {
                vm.AddTodo(todos.ToList());
            }
        }

        private void AssociatedObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }
    }
}