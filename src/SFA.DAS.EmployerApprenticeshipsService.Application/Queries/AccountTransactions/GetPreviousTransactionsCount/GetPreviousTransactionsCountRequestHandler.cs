using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequestHandler : IAsyncRequestHandler<GetPreviousTransactionsCountRequest, GetPreviousTransactionsCountResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IValidator<GetPreviousTransactionsCountRequest> _validator;

        public GetPreviousTransactionsCountRequestHandler(IDasLevyRepository repository, IValidator<GetPreviousTransactionsCountRequest> validator)
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
