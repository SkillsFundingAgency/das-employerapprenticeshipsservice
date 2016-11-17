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
                return GetResponse(message.HashedId, message.AccountId);
            }

            var transactionSummaries = new List<TransactionLine>();

            foreach (var transaction in response)
            {

                var description = "";
                if (transaction.TransactionType == LevyItemType.Declaration)
                {
                    description = transaction.Amount >= 0 ? "Credit" : "Adjustment";
                }
                else if (transaction.TransactionType == LevyItemType.Payment)
                {
                    string providerName;
                    try
                    {
                        var provider = _apprenticeshipInfoServiceWrapper.GetProvider(Convert.ToInt32(transaction.UkPrn));
                        providerName = provider.Providers[0].ProviderName;
                    }
                    catch (Exception ex)
                    {
                        providerName = "Unknown provider";
                        _logger.Info(ex, $"Provider not found for UkPrn:{transaction.UkPrn}");
                    }
                    
                    description = providerName;
                }

                var transactionLine = new TransactionLine
                {
                    UkPrn = transaction.UkPrn,
                    Amount = transaction.Amount,
                    TransactionDate = transaction.TransactionDate,
                    Description = description,
                    Balance = transaction.Balance
                };

                transactionSummaries.Add(transactionLine);
            }
            
            return GetResponse(message.HashedId, message.AccountId, transactionSummaries);
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
                    HashedId = hashedAccountId,
                    AccountId = accountId,
                    TransactionLines = transactions
                }
            };
        }
        
    }
}