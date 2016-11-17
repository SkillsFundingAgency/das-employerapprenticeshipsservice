using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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

        public GetEmployerAccountTransactionsHandler(IDasLevyService dasLevyService, IValidator<GetEmployerAccountTransactionsQuery> validator, IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            _dasLevyService = dasLevyService;
            _validator = validator;
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
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
                if (transaction.GetType() == typeof(LevyDeclarationTransactionLine))
                {
                    transaction.Description = transaction.Amount >= 0 ? "Credit" : "Adjustment";
                }
                else if (transaction.GetType() == typeof(PaymentTransactionLine))
                {
                    var providerName = _apprenticeshipInfoServiceWrapper.GetProvider(
                        Convert.ToInt32(((PaymentTransactionLine)transaction).UkPrn));

                    transaction.Description = $"Payment to provider {providerName.Providers[0].ProviderName}";
                }

                transactionSummaries.Add(transaction);
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