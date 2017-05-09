using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions
{
    public class GetAccountProviderTransactionsHandler : IAsyncRequestHandler<GetAccountProviderTransactionsQuery, GetAccountProviderTransactionsResponse>
    {
        private readonly IValidator<GetAccountProviderTransactionsQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IHashingService _hashingService;

        public GetAccountProviderTransactionsHandler(
            IValidator<GetAccountProviderTransactionsQuery> validator,
            IDasLevyService dasLevyService,
            IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _hashingService = hashingService;
        }

        public async Task<GetAccountProviderTransactionsResponse> Handle(GetAccountProviderTransactionsQuery message)
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
            var transactions = await _dasLevyService.GetAccountProviderTransactionsByDateRange<PaymentTransactionLine>
                                    (accountId, message.FromDate, message.ToDate, message.ExternalUserId);

            if (!transactions.Any())
            {
                throw new NotFoundException("No transactions found.");
            }

            var firstTransaction = transactions.First();

            return new GetAccountProviderTransactionsResponse
            {
                ProviderName = firstTransaction.ProviderName,
                TransactionDate = firstTransaction.TransactionDate,
                Transactions = transactions.ToList(),
                Total = transactions.Sum(c => c.LineAmount)
            };
        }
    }
}