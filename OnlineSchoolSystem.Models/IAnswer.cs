using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    public interface IAnswer
    {
        public string QuestionId { get; }
        public Message Message { get; set; }
    }
}
