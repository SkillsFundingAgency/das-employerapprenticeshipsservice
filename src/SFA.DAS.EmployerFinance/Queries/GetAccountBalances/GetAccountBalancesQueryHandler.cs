﻿using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountBalances
{
    public class GetAccountBalancesQueryHandler : IAsyncRequestHandler<GetAccountBalancesRequest, GetAccountBalancesResponse>
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IValidator<GetAccountBalancesRequest> _validator;

        public GetAccountBalancesQueryHandler(IDasLevyRepository dasLevyRepository, IValidator<GetAccountBalancesRequest> validator)
        {
            _dasLevyRepository = dasLevyRepository;
            _validator = validator;
        }

        public async Task<GetAccountBalancesResponse> Handle(GetAccountBalancesRequest message)
        {

            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _dasLevyRepository.GetAccountBalances(message.AccountIds);

            return new GetAccountBalancesResponse { Accounts = result };
        }
    }
}