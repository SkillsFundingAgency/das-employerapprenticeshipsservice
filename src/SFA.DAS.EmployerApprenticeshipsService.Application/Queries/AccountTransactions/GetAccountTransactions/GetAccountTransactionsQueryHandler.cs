using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsQueryHandler : IAsyncRequestHandler<GetAccountTransactionsRequest, GetAccountTransactionsResponse>
    {
        private readonly IValidator<GetAccountTransactionsRequest> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        
        public GetAccountTransactionsQueryHandler(IValidator<GetAccountTransactionsRequest> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetAccountTransactionsResponse> Handle(GetAccountTransactionsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _dasLevyRepository.GetTransactions(message.AccountId);

            return new GetAccountTransactionsResponse {TransactionLines = result};
        }
    }
}