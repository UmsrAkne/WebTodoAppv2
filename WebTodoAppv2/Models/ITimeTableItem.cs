namespace WebTodoAppv2.Models
{
    using System;

    public interface ITimeTableItem
    {
        DateTime DateTime { get; set; }

        string Text { get; set; }
    }
}
