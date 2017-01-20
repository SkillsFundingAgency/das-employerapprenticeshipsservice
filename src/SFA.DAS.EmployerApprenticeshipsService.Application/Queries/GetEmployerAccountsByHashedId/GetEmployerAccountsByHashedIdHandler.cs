using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId
{
    public class GetEmployerAccountsByHashedIdHandler : IAsyncRequestHandler<GetEmployerAccountsByHashedIdQuery, GetEmployerAccountsByHashedIdResponse>
    {
        private readonly IValidator<GetEmployerAccountsByHashedIdQuery> _validator;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetEmployerAccountsByHashedIdHandler(IValidator<GetEmployerAccountsByHashedIdQuery> validator, IEmployerAccountRepository employerAccountRepository)
        {
            _validator = validator;
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetEmployerAccountsByHashedIdResponse> Handle(GetEmployerAccountsByHashedIdQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var results = await _employerAccountRepository.GetAccountsInformationByHashedId(message.HashedAccountId);

            return new GetEmployerAccountsByHashedIdResponse {Accounts = results };
        }
    }
}
