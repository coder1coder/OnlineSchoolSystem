using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    public class Message
    {
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public AuthorDetails AuthorDetails { get; set; }

    }
}
