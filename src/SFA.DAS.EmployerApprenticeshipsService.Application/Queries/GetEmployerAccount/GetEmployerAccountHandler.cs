using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHandler : IAsyncRequestHandler<GetEmployerAccountQuery, GetEmployerAccountResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository   ;
        private readonly IValidator<GetEmployerAccountQuery> _validator;

        public GetEmployerAccountHandler(IEmployerAccountRepository employerAccountRepository, IValidator<GetEmployerAccountQuery> validator)
        {
            if (employerAccountRepository == null)
                throw new ArgumentNullException(nameof(employerAccountRepository));
            _employerAccountRepository = employerAccountRepository;
            _validator = validator;
        }

        public async Task<GetEmployerAccountResponse> Handle(GetEmployerAccountQuery message)
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
            

            var employerAccount = await _employerAccountRepository.GetAccountById(message.AccountId);

            return new GetEmployerAccountResponse
            {
                Account = employerAccount
            };
        }
    }
}
