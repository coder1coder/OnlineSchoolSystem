using OnlineSchoolSystem.Models;
using System.Threading.Tasks;

namespace OnlineSchoolSystem.Bots.Models
{
    public interface IBot
    {
        BotState State { get; set; }

        Task StartAsync(IStorage storage, ISettings settings);
        void Stop();
    }
}
