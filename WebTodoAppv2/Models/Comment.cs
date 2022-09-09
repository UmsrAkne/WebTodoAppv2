namespace WebTodoAppv2.Models
{
    using System;

    public class Comment
    {
        public int Id { get; set; }

        public int TodoId { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; }
    }
}
