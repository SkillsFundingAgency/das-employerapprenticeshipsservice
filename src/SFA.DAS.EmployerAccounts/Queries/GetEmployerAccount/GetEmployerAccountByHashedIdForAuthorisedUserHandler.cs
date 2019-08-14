using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdForAuthorisedUserHandler : IAsyncRequestHandler<GetEmployerAccountByHashedIdForAuthorisedUserQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IValidator<GetEmployerAccountByHashedIdForAuthorisedUserQuery> _validator;

        public GetEmployerAccountByHashedIdForAuthorisedUserHandler(
            IEmployerAccountRepository employerAccountRepository,
            IValidator<GetEmployerAccountByHashedIdForAuthorisedUserQuery> validator)
        {
            _employerAccountRepository = employerAccountRepository;
            _validator = validator;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountByHashedIdForAuthorisedUserQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var employerAccount = await _employerAccountRepository.GetAccountByHashedId(message.HashedAccountId);

            return new GetEmployerAccountResponse
            {
                Account = employerAccount
            };
        }
    }
}