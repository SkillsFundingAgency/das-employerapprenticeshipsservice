using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    public class FindEmployerAccountLevyDeclarationTransactionsHandler : IAsyncRequestHandler<FindEmployerAccountLevyDeclarationTransactionsQuery, FindEmployerAccountLevyDeclarationTransactionsResponse>
    {
        private readonly IValidator<FindEmployerAccountLevyDeclarationTransactionsQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IHashingService _hashingService;

        public FindEmployerAccountLevyDeclarationTransactionsHandler(
            IValidator<FindEmployerAccountLevyDeclarationTransactionsQuery> validator, 
            IDasLevyService dasLevyService,
            IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _hashingService = hashingService;
        }

        public async Task<FindEmployerAccountLevyDeclarationTransactionsResponse> Handle(FindEmployerAccountLevyDeclarationTransactionsQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var transactions = await _dasLevyService.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (accountId, message.FromDate, message.ToDate, message.ExternalUserId);
            
            //var transactionDetailSummaries = data.Select(item => new LevyDeclarationTransactionLine
            //{
            //    Amount = item.Amount,
            //    EmpRef = item.EmpRef,
            //    TopUp = item.TopUp,
            //    TransactionDate = item.TransactionDate,
            //    EnglishFraction = item.EnglishFraction,
            //    LineAmount = item.LineAmount
            //}).ToList();

            return new FindEmployerAccountLevyDeclarationTransactionsResponse
            {
                Transactions = transactions.ToList(),
                Total = transactions.Sum(c => c.LineAmount)
            };
        }
    }
}