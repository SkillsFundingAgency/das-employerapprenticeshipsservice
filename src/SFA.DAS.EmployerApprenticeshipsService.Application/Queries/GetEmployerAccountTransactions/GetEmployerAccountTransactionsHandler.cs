using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
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

            var response = await _dasLevyService.GetTransactionsByAccountId(message.AccountId);

            if (!response.Any())
            {
                return GetResponse(message.HashedAccountId, message.AccountId);
            }

            var transactionSummaries = new List<TransactionLine>();

            foreach (var transaction in response)
            {
                if (transaction.GetType() == typeof(LevyDeclarationTransactionLine))
                {
                    transaction.Description = transaction.Amount >= 0 ? "Credit" : "Adjustment";
                }
                else if (transaction.GetType() == typeof(PaymentTransactionLine))
                {
                    var paymentTransaction = (PaymentTransactionLine) transaction;
                    
                    try
                    {
                    	var providerName = _apprenticeshipInfoServiceWrapper.GetProvider(
                        Convert.ToInt32(paymentTransaction.UkPrn));

                    	transaction.Description = $"Payment to provider {providerName.Provider.ProviderName}";
                    }
                    catch (Exception ex)
                    {
                        transaction.Description = "Training provider - name not recognised";
                        _logger.Info(ex, $"Provider not found for UkPrn:{paymentTransaction.UkPrn}");
                    }                     
                }

                transactionSummaries.Add(transaction);
            }
            
            return GetResponse(message.HashedAccountId, message.AccountId, transactionSummaries);
        }


        private static GetEmployerAccountTransactionsResponse GetResponse(string hashedAccountId, long accountId)
        {
            return GetResponse(hashedAccountId, accountId, new List<TransactionLine>());
        }

        private static GetEmployerAccountTransactionsResponse GetResponse(
            string hashedAccountId, long accountId, ICollection<TransactionLine> transactions)
        {
            return new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData
                {
                    HashedAccountId = hashedAccountId,
                    AccountId = accountId,
                    TransactionLines = transactions
                }
            };
        }
        
    }
}