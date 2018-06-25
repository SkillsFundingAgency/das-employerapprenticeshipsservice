using NServiceBus;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Messages.Commands;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Services
{
    public class RefreshEmployerLevyService : IRefreshEmployerLevyService
    {
        private readonly IEndpointInstance _endpoint;


        public RefreshEmployerLevyService(IEndpointInstance endpoint)
        {
            _endpoint = endpoint;
        }

        public Task QueueRefreshLevyMessage(long accountId, string payeRef)
        {
            return _endpoint.Send<ImportAccountLevyDeclarationsCommand>(c =>
            {
                c.AccountId = accountId;
                c.PayeRef = payeRef;
            });
        }
    }
}
