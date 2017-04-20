using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.UpdatePayeInformation;
using SFA.DAS.EAS.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DasAccountService : IDasAccountService
    {
        private readonly IMediator _mediator;

        public DasAccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<PayeSchemes> GetAccountSchemes(long accountId)
        {
            var getEmployerSchemesResponse = await _mediator.SendAsync(new GetEmployerSchemesQuery { Id = accountId });
            return getEmployerSchemesResponse.PayeSchemes;
        }

        public async Task UpdatePayeScheme(string empRef)
        {
            await _mediator.SendAsync(new UpdatePayeInformationCommand {PayeRef = empRef});
        }
    }
}