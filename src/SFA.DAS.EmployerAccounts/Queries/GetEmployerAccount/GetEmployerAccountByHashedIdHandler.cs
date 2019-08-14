using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdHandler : IAsyncRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IValidator<GetEmployerAccountByHashedIdQuery> _validator;

        public GetEmployerAccountByHashedIdHandler(
            IEmployerAccountRepository employerAccountRepository,
            IValidator<GetEmployerAccountByHashedIdQuery> validator)
        {
            _employerAccountRepository = employerAccountRepository;
            _validator = validator;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountByHashedIdQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var employerAccount = await _employerAccountRepository.GetAccountByHashedId(message.HashedAccountId);

            return new GetEmployerAccountResponse
            {
                Account = employerAccount
            };
        }
    }
}