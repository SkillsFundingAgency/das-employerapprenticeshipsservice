using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.Commands.AuditCommand
{
    public class AuditCommand : IAsyncRequest
    {
        public EasAuditMessage EasAuditMessage { get; set; }
    }
}
