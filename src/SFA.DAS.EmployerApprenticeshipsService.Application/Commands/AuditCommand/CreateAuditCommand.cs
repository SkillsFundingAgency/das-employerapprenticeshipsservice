using MediatR;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.Commands.AuditCommand
{
    public class CreateAuditCommand : IAsyncRequest
    {
        public EasAuditMessage EasAuditMessage { get; set; }
    }
}
