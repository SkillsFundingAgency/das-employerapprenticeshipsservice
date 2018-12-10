using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Queries.GetEnglishFrationDetail;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;

        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef)
        {
            var result = await _mediator.SendAsync(new GetEnglishFractionDetailByEmpRefQuery
            {
                AccountId = accountId,
                EmpRef = empRef
            });

            return result.FractionDetail;
        }
    }
}