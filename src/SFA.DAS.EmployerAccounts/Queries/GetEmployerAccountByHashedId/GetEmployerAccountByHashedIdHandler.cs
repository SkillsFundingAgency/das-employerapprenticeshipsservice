using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdHandler : IAsyncRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>
    {
        private readonly IValidator<GetEmployerAccountByHashedIdQuery> _validator;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetEmployerAccountByHashedIdHandler(IValidator<GetEmployerAccountByHashedIdQuery> validator, IEmployerAccountRepository employerAccountRepository)
        {
            _validator = validator;
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetEmployerAccountByHashedIdResponse> Handle(GetEmployerAccountByHashedIdQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var account = await _employerAccountRepository.GetAccountDetailByHashedId(message.HashedAccountId);

            return new GetEmployerAccountByHashedIdResponse {Account = account};
        }
    }
}
