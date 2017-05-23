using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsHandler :
        IAsyncRequestHandler<GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private readonly IDasLevyService _dasLevyService;
        private readonly IValidator<GetEmployerAccountTransactionsQuery> _validator;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;
        private readonly ILogger _logger;

        public GetEmployerAccountTransactionsHandler(IDasLevyService dasLevyService, IValidator<GetEmployerAccountTransactionsQuery> validator, IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper, ILogger logger)
        {
            _dasLevyService = dasLevyService;
            _validator = validator;
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
            _logger = logger;
        }

        public async Task<GetEmployerAccountTransactionsResponse> Handle(GetEmployerAccountTransactionsQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var transactions = await _dasLevyService.GetAccountTransactionsByDateRange(message.AccountId, message.FromDate, message.ToDate);

            var hasPreviousTransactions = await _dasLevyService.GetPreviousAccountTransaction(message.AccountId, message.FromDate, message.ExternalUserId) > 0;
            
            if (!transactions.Any())
            {
                return GetResponse(message.HashedAccountId, message.AccountId, hasPreviousTransactions);
            }
            
            foreach (var transaction in transactions)
            {
                GenerateTransactionDescription(transaction);
            }
            
            return GetResponse(message.HashedAccountId, message.AccountId, transactions, hasPreviousTransactions);
        }

        private void GenerateTransactionDescription(TransactionLine transaction)
        {
            if (transaction.GetType() == typeof(LevyDeclarationTransactionLine))
            {
                transaction.Description = transaction.Amount >= 0 ? "Levy" : "Levy adjustment";
            }
            else if (transaction.GetType() == typeof(PaymentTransactionLine))
            {
                var paymentTransaction = (PaymentTransactionLine)transaction;

                transaction.Description = GetPaymentTransactionDescription(paymentTransaction);
            }
        }

        private string GetPaymentTransactionDescription(PaymentTransactionLine transaction)
        {
            var transactionPrefix = transaction.IsCoInvested ? "Co-investment - " : string.Empty;

            try
            {
                var ukprn = Convert.ToInt32(transaction.UkPrn);
                var providerName = _apprenticeshipInfoServiceWrapper.GetProvider(ukprn);

                return $"{transactionPrefix}{providerName.Provider.ProviderName}";
            }
            catch (Exception ex)
            {
                _logger.Info(ex, $"Provider not found for UkPrn:{transaction.UkPrn}");
            }

            return $"{transactionPrefix}Training provider - name not recognised";
        }

        private static GetEmployerAccountTransactionsResponse GetResponse(string hashedAccountId, long accountId, bool hasPreviousTransactions)
        {
            return GetResponse(hashedAccountId, accountId, new List<TransactionLine>(), hasPreviousTransactions);
        }

        private static GetEmployerAccountTransactionsResponse GetResponse(
            string hashedAccountId, long accountId, ICollection<TransactionLine> transactions, bool hasPreviousTransactions)
        {
            return new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData
                {
                    HashedAccountId = hashedAccountId,
                    AccountId = accountId,
                    TransactionLines = transactions
                },
                AccountHasPreviousTransactions = hasPreviousTransactions
                
            };
        }
    }
}