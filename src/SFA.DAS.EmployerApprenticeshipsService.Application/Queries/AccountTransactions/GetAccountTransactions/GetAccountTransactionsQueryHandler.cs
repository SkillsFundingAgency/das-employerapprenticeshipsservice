using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsQueryHandler : IAsyncRequestHandler<GetAccountTransactionsRequest, GetAccountTransactionsResponse>
    {
        private readonly IValidator<GetAccountTransactionsRequest> _validator;
        private readonly ITransactionRepository _transactionRepository;
        
        public GetAccountTransactionsQueryHandler(IValidator<GetAccountTransactionsRequest> validator, ITransactionRepository transactionRepository)
        {
            _validator = validator;
            _transactionRepository = transactionRepository;
        }

        public async Task<GetAccountTransactionsResponse> Handle(GetAccountTransactionsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _transactionRepository.GetAccountTransactionsByDateRange(message.AccountId, message.FromDate, message.ToDate);

            return new GetAccountTransactionsResponse {TransactionLines = result};
        }
    }
}