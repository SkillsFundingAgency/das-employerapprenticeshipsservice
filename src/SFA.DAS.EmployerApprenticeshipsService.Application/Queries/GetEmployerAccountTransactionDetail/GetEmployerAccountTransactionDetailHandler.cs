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
    public class GetEmployerAccountTransactionDetailHandler : IAsyncRequestHandler<GetEmployerAccountTransactionDetailQuery, GetEmployerAccountTransactionDetailResponse>
    {
        private readonly IValidator<GetEmployerAccountTransactionDetailQuery> _validator;
        private readonly IDasLevyService _dasLevyService;

        public GetEmployerAccountTransactionDetailHandler(IValidator<GetEmployerAccountTransactionDetailQuery> validator, IDasLevyService dasLevyService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
        }

        public async Task<GetEmployerAccountTransactionDetailResponse> Handle(GetEmployerAccountTransactionDetailQuery message)
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

            var data = await _dasLevyService.GetTransactionDetailById(message.Id);


            var transactionDetailSummary = data.GroupBy(c => new { c.SubmissionId }, (submission, group) => new
            {
                submission.SubmissionId,
                Data = group.ToList()
            }).Select(item =>
            {
                return new TransactionDetailSummary
                {
                    Amount = item.Data.Where(c=>c.TransactionType.Equals(LevyItemType.Declaration)).Sum(c => c.Amount),
                    Empref = item.Data.First().EmpRef,
                    TopUp = item.Data.Where(c => c.TransactionType.Equals(LevyItemType.TopUp)).Sum(c => c.Amount),
                    TransactionDate = item.Data.First().TransactionDate,
                    EnglishFraction = item.Data.First().EnglishFraction
                };
            }).ToList();
            var totalAmount = transactionDetailSummary.Sum(c => c.Amount);
            return new GetEmployerAccountTransactionDetailResponse {TransactionDetail = transactionDetailSummary,Total = totalAmount };

        }
    }
}