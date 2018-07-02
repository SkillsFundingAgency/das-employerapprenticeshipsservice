using NServiceBus;
using SFA.DAS.EAS.Domain.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EAS.Application.Services
{
    public class RefreshEmployerLevyService : IRefreshEmployerLevyService
    {
        private readonly IMessageSession _messageSession;

        public RefreshEmployerLevyService(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public Task QueueRefreshLevyMessage(long accountId, string payeRef)
        {
            return _messageSession.Send<ImportAccountLevyDeclarationsCommand>(c =>
            {
                c.AccountId = accountId;
                c.PayeRef = payeRef;
            });
        }
    }
}
