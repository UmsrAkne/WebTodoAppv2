using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace WebTodoAppv2.Models
{
    public class Group : BindableBase
    {
        private string name;
        private bool editMode;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }

        public string Name { get => name; set => SetProperty(ref name, value); }

        [NotMapped]
        public bool EditMode { get => editMode; set => SetProperty(ref editMode, value); }
    }
}