using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountDetail
{
    public class GetEmployerAccountDetailByHashedIdQueryHandler : IAsyncRequestHandler<GetEmployerAccountDetailByHashedIdQuery, GetEmployerAccountDetailByHashedIdResponse>
    {
        private readonly IValidator<GetEmployerAccountDetailByHashedIdQuery> _validator;
        private readonly IAccountApiClient _accountApiClient;
        private readonly IMapper _mapper;

        public GetEmployerAccountDetailByHashedIdQueryHandler(IValidator<GetEmployerAccountDetailByHashedIdQuery> validator, IAccountApiClient accountApiClient, IMapper mapper)
        {
            _validator = validator;
            _accountApiClient = accountApiClient;
            _mapper = mapper;
        }

        public async Task<GetEmployerAccountDetailByHashedIdResponse> Handle(GetEmployerAccountDetailByHashedIdQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountDetail = await _accountApiClient.GetAccount(message.HashedAccountId);

            return new GetEmployerAccountDetailByHashedIdResponse 
            { 
                AccountDetail = _mapper.Map<AccountDetailDto>(accountDetail) 
            };
        }
    }
}
