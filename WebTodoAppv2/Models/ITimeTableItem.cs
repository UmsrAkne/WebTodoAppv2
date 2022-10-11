using System;

namespace WebTodoAppv2.Models
{
    public interface ITimeTableItem
    {
        DateTime DateTime { get; set; }

        string Text { get; set; }
    }
}