using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler : IAsyncRequestHandler<GetAccountTransactionsByDateRangeQuery,GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private readonly IValidator<GetAccountTransactionsByDateRangeQuery> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;


        public GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(IValidator<GetAccountTransactionsByDateRangeQuery> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetAccountLevyDeclationTransactionsByDateRangeResponse> Handle(GetAccountTransactionsByDateRangeQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);
            
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transactions = await _dasLevyRepository.GetTransactionsByDateRange(message.AccountId, message.FromDate,
                message.ToDate);

            var levyDeclarationTransactions = transactions.OfType<LevyDeclarationTransactionLine>().ToList();

            return new GetAccountLevyDeclationTransactionsByDateRangeResponse { Transactions = levyDeclarationTransactions };
        }
    }
}
