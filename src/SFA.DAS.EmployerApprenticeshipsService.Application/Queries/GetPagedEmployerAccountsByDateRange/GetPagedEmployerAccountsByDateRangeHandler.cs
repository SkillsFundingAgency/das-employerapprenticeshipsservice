using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange
{
    public class GetPagedEmployerAccountsByDateRangeHandler : IAsyncRequestHandler<GetPagedEmployerAccountsByDateRangeQuery, GetPagedEmployerAccountsByDateRangeResponse>
    {
        private readonly IValidator<GetPagedEmployerAccountsByDateRangeQuery> _validator;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetPagedEmployerAccountsByDateRangeHandler(IValidator<GetPagedEmployerAccountsByDateRangeQuery> validator, IEmployerAccountRepository employerAccountRepository)
        {
            _validator = validator;
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetPagedEmployerAccountsByDateRangeResponse> Handle(GetPagedEmployerAccountsByDateRangeQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var results = await _employerAccountRepository.GetAccountsByDateRange(message.FromDate, message.ToDate, message.PageNumber,message.PageSize);

            return new GetPagedEmployerAccountsByDateRangeResponse {Accounts = results };
        }
    }
}
