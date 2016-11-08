using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionDetailQueryHandler : IAsyncRequestHandler<GetAccountTransactionDetailQuery,GetAccountTransactionDetailResponse>
    {
        private readonly IValidator<GetAccountTransactionDetailQuery> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetAccountTransactionDetailQueryHandler(IValidator<GetAccountTransactionDetailQuery> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetAccountTransactionDetailResponse> Handle(GetAccountTransactionDetailQuery message)
        {
            var validationResult = _validator.Validate(message);

            //TODO do validation for the account

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var response = await _dasLevyRepository.GetTransactionDetail(message.Id);

            return new GetAccountTransactionDetailResponse { Data = response};
        }
        
    }

}
