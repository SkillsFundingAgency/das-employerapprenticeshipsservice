using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface INotificationRepository
    {
        Task<int> Create(NotificationMessage message);
    }
}