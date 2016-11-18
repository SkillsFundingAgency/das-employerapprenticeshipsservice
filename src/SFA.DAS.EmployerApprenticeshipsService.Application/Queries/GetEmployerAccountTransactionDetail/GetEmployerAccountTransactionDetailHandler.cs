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
    public class GetEmployerAccountTransactionDetailHandler : IAsyncRequestHandler<GetAccountLevyDeclarationTransactionsByDateRangeQuery, GetAccountLevyDeclarationTransactionsByDateRangeResponse>
    {
        private readonly IValidator<GetAccountLevyDeclarationTransactionsByDateRangeQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IHashingService _hashingService;

        public GetEmployerAccountTransactionDetailHandler(
            IValidator<GetAccountLevyDeclarationTransactionsByDateRangeQuery> validator, 
            IDasLevyService dasLevyService,
            IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _hashingService = hashingService;
        }

        public async Task<GetAccountLevyDeclarationTransactionsByDateRangeResponse> Handle(GetAccountLevyDeclarationTransactionsByDateRangeQuery message)
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

            var transactionSubmissionItems = data.GroupBy(c => 
                new { c.SubmissionId }, (submission, group) => 
                   new{
                        submission.SubmissionId,
                        Data = group.ToList()
                       });

            var transactionDetailSummaries = transactionSubmissionItems.Select(item =>
            {
                var amount = item.Data.Where(c=>c.TransactionType.Equals(TransactionItemType.Declaration)).Sum(c => c.Amount);
                var topUp = item.Data.Where(c => c.TransactionType.Equals(TransactionItemType.TopUp)).Sum(c => c.Amount);

                return new LevyDeclarationTransactionLine
                {
                    Amount = amount,
                    EmpRef = item.Data.First().EmpRef,
                    TopUp = topUp,
                    TransactionDate = item.Data.First().TransactionDate,
                    EnglishFraction = item.Data.First().EnglishFraction,
                    LineTotal = amount + topUp
                };
            }).ToList();
            
            return new GetAccountLevyDeclarationTransactionsByDateRangeResponse
            {
                TransactionDetail = transactionDetailSummaries,
                Total = transactionDetailSummaries.Sum(c => c.LineTotal)
            };
        }
    }
}