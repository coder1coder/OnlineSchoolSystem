using OnlineSchoolSystem.Models;

namespace OnlineSchoolSystem.Bots.Models
{
    public interface IBot
    {
        BotState State { get; set; }

        void Start(IStorage storage);
        void Stop();
    }
}
