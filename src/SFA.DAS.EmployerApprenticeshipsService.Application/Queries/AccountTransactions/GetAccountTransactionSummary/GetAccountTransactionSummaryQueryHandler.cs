//using System.Threading.Tasks;
//using MediatR;
//using SFA.DAS.EAS.Domain.Data.Repositories;
//using SFA.DAS.HashingService;

//namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionSummary
//{
//    public class GetAccountTransactionSummaryQueryHandler : IAsyncRequestHandler<GetAccountTransactionSummaryRequest, GetAccountTransactionSummaryResponse>
//    {
//        private readonly IHashingService _hashingService;
//        private readonly ITransactionRepository _transactionRepository;
        
//        public GetAccountTransactionSummaryQueryHandler(IHashingService hashingService, ITransactionRepository transactionRepository)
//        {
//            _hashingService = hashingService;
//            _transactionRepository = transactionRepository;
//        }

//        public async Task<GetAccountTransactionSummaryResponse> Handle(GetAccountTransactionSummaryRequest message)
//        {
//            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
//            var result = await _transactionRepository.GetAccountTransactionSummary(accountId);

//            return new GetAccountTransactionSummaryResponse { Data = result };
//        }
//    }
//}