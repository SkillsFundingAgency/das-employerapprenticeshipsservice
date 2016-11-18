using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailHandler : IAsyncRequestHandler<GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery, GetEmployerAccountLevyDeclarationTransactionsByDateRangeResponse>
    {
        private readonly IValidator<GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IHashingService _hashingService;

        public GetEmployerAccountTransactionDetailHandler(
            IValidator<GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery> validator, 
            IDasLevyService dasLevyService,
            IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _hashingService = hashingService;
        }

        public async Task<GetEmployerAccountLevyDeclarationTransactionsByDateRangeResponse> Handle(GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery message)
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
            var data = await _dasLevyService.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                    (accountId, message.FromDate, message.ToDate, message.ExternalUserId);
            
            var transactionDetailSummaries = data.Select(item => new LevyDeclarationTransactionLine
            {
                Amount = item.Amount,
                EmpRef = item.EmpRef,
                TopUp = item.TopUp,
                TransactionDate = item.TransactionDate,
                EnglishFraction = item.EnglishFraction,
                LineAmount = item.LineAmount
            }).ToList();

            return new GetEmployerAccountLevyDeclarationTransactionsByDateRangeResponse
            {
                Transactions = transactionDetailSummaries,
                Total = transactionDetailSummaries.Sum(c => c.LineAmount)
            };
        }
    }
}