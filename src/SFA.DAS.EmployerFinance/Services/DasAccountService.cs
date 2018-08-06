using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Commands.UpdatePayeInformation;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.EmployerFinance.Queries.GetEmployerSchemes;

namespace SFA.DAS.EmployerFinance.Services
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