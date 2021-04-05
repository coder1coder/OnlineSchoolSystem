using System.Collections.Generic;

namespace OnlineSchoolSystem.Models
{
    //Статистика должна быть абстрактной относительно сборщиков
    /// <summary>
    /// модель для методов сбора статистики
    /// </summary>
    /// 
    public class StatisticPerStream
    {
        public string LiveChatId { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public IEnumerable<IAnswer> Answers { get; set; }
        public IEnumerable<IQuestion> Questions { get; set; }        

    }
}
