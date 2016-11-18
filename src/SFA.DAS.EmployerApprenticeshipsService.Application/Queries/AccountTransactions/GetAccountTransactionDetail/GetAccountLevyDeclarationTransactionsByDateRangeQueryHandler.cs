using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler : IAsyncRequestHandler<GetAccountTransactionsByDateRangeQuery,GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private readonly IValidator<GetAccountTransactionsByDateRangeQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
       
        public GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(IValidator<GetAccountTransactionsByDateRangeQuery> validator, IDasLevyService dasLevyService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
        }

        public async Task<GetAccountLevyDeclationTransactionsByDateRangeResponse> Handle(GetAccountTransactionsByDateRangeQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);
            
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var response = await _dasLevyService.GetTransactionsByDateRange<LevyDeclarationTransactionLine>(
                message.AccountId, message.FromDate, message.ToDate, message.ExternalUserId);

            return new GetAccountLevyDeclationTransactionsByDateRangeResponse { Transactions = response.ToList() };
        }
    }
}
