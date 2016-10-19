using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetBatchEmployerAccountTransactions
{
    public class GetBatchEmployerAccountTransactionsHandler : IAsyncRequestHandler<GetBatchEmployerAccountTransactionsQuery, GetBatchEmployerAccountTransactionsResponse>
    {
        private readonly IAggregationRepository _aggregationRepository;

        public GetBatchEmployerAccountTransactionsHandler(IAggregationRepository aggregationRepository)
        {
            _aggregationRepository = aggregationRepository;
        }



        public async Task<GetBatchEmployerAccountTransactionsResponse> Handle(GetBatchEmployerAccountTransactionsQuery message)
        {
            var response = await _aggregationRepository.GetByAccountIds(message.AccountIds);

            return new GetBatchEmployerAccountTransactionsResponse() { Data = response };
        }
    }
}
