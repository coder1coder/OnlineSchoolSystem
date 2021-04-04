using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{/// <summary>
/// модель для методов сбора статистики
/// </summary>
    public class StatisticPerStream
    {
        public string LiveChatId { get; set; }
        public List<AuthorDetails> Authors { get; set; }
        public List<IAnswer> Answers { get; set; }
        public List<IQuestion> Questions { get; set; }        

    }
}
