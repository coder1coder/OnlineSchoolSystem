using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    public interface IQuestion
    {
        public string Id { get; set; }
        public Message Message { get; set; }

    }
}
