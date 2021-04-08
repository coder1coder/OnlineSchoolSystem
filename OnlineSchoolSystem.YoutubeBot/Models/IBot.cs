using System.Threading.Tasks;

namespace OnlineSchoolSystem.Bots.Models
{
    public interface IBot
    {
        TaskStatus Status { get; }

        bool CanStart();
        bool Start();
        void Stop();
    }
}
