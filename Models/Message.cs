using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Message
    {
        public Message()
        {}

        public Message(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
    }
}
