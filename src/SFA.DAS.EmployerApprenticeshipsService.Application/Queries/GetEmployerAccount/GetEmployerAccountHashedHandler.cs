using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHashedHandler : IAsyncRequestHandler<GetEmployerAccountHashedQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IValidator<GetEmployerAccountHashedQuery> _validator;

        public GetEmployerAccountHashedHandler(IEmployerAccountRepository employerAccountRepository, IValidator<GetEmployerAccountHashedQuery> validator)
        {
            _employerAccountRepository = employerAccountRepository;
            _validator = validator;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountHashedQuery message)
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