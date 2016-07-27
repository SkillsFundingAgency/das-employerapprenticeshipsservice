using System.Threading.Tasks;

namespace SFA.DAS.EAS.Notification.Worker.Providers
{
    public interface INotification
    {
        Task Handle();
    }
}