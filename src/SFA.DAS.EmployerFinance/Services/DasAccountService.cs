using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Commands.UpdatePayeInformation;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasAccountService : IDasAccountService
    {
        private readonly IMediator _mediator;

        public DasAccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task UpdatePayeScheme(string empRef)
        {
            await _mediator.SendAsync(new UpdatePayeInformationCommand {PayeRef = empRef});
        }
    }
}