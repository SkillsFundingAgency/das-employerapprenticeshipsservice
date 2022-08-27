using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryQueryHandler : IAsyncRequestHandler<GetAccountTransactionSummaryRequest, GetAccountTransactionSummaryResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly ITransactionRepository _transactionRepository;

        public GetAccountTransactionSummaryQueryHandler(IHashingService hashingService, ITransactionRepository transactionRepository)
        {
            _hashingService = hashingService;
            _transactionRepository = transactionRepository;
        }

        public async Task<GetAccountTransactionSummaryResponse> Handle(GetAccountTransactionSummaryRequest message)
        {
            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var result = await _transactionRepository.GetAccountTransactionSummary(accountId);

            return new GetAccountTransactionSummaryResponse { Data = result };
        }
    }
}
