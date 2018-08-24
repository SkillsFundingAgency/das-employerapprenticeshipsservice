using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Exceptions;
using SFA.DAS.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequestHandler : IAsyncRequestHandler<GetPreviousTransactionsCountRequest, GetPreviousTransactionsCountResponse>
    {
        private readonly ITransactionRepository _repository;
        private readonly IValidator<GetPreviousTransactionsCountRequest> _validator;

        public GetPreviousTransactionsCountRequestHandler(ITransactionRepository repository, IValidator<GetPreviousTransactionsCountRequest> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GetPreviousTransactionsCountResponse> Handle(GetPreviousTransactionsCountRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transactionCount = await _repository.GetPreviousTransactionsCount(message.AccountId, message.FromDate);

            return new GetPreviousTransactionsCountResponse
            {
                Count = transactionCount
            };
        }
    }
}
