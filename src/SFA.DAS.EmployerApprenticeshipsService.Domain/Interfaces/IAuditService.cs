using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAuditService
    {
        Task SendAuditMessage(EasAuditMessage message);
    }
}