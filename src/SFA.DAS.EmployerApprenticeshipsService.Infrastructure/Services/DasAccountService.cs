using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DasAccountService : IDasAccountService
    {
        private readonly IMediator _mediator;

        public DasAccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Schemes> GetAccountSchemes(long accountId)
        {
            var getEmployerSchemesResponse = await _mediator.SendAsync(new GetEmployerSchemesQuery { Id = accountId });
            return getEmployerSchemesResponse.Schemes;
        }
    }
}