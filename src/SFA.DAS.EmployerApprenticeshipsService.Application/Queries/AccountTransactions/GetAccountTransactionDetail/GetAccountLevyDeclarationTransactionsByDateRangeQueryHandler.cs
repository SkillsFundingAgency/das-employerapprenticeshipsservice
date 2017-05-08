using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler : IAsyncRequestHandler<GetAccountTransactionsByDateRangeQuery,GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        private readonly IValidator<GetAccountTransactionsByDateRangeQuery> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IHmrcDateService _hmrcDateService;


        public GetAccountLevyDeclarationTransactionsByDateRangeQueryHandler(IValidator<GetAccountTransactionsByDateRangeQuery> validator, IDasLevyRepository dasLevyRepository, IHmrcDateService hmrcDateService)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _hmrcDateService = hmrcDateService;
        }

        public async Task<GetAccountLevyDeclationTransactionsByDateRangeResponse> Handle(GetAccountTransactionsByDateRangeQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);
            
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transactions = await _dasLevyRepository.GetTransactionDetailsByDateRange(message.AccountId, message.FromDate,
                message.ToDate);

            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction.PayrollYear) && transaction.PayrollMonth != 0)
                {
                    transaction.PayrollDate = _hmrcDateService.GetDateFromPayrollYearMonth(transaction.PayrollYear, transaction.PayrollMonth);
                }
            }

            return new GetAccountLevyDeclationTransactionsByDateRangeResponse { Transactions = transactions };
        }
    }
}
