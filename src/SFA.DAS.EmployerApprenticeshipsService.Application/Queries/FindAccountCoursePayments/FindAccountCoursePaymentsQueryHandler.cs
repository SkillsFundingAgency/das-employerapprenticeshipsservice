using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Application.Queries.FindAccountCoursePayments
{
    public class FindAccountCoursePaymentsQueryHandler : IAsyncRequestHandler<FindAccountCoursePaymentsQuery,
        FindAccountCoursePaymentsResponse>
    {
        private readonly IValidator<FindAccountCoursePaymentsQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IHashingService _hashingService;

        public FindAccountCoursePaymentsQueryHandler(
            IValidator<FindAccountCoursePaymentsQuery> validator,
            IDasLevyService dasLevyService,
            IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _hashingService = hashingService;
        }

        public async Task<FindAccountCoursePaymentsResponse> Handle(FindAccountCoursePaymentsQuery message)
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
            var transactions = await _dasLevyService.GetAccountCoursePaymentsByDateRange<PaymentTransactionLine>
            (accountId, message.UkPrn, message.CourseName, message.FromDate, message.ToDate,
                message.ExternalUserId);

            if (!transactions.Any())
            {
                throw new NotFoundException("No transactions found.");
            }

            var firstTransaction = transactions.First();

            return new FindAccountCoursePaymentsResponse
            {
                ProviderName = firstTransaction.ProviderName,
                CourseName = firstTransaction.CourseName,
                CourseLevel = firstTransaction.CourseLevel,
                TransactionDate = firstTransaction.TransactionDate,
                Transactions = transactions.ToList(),
                Total = transactions.Sum(c => c.LineAmount)
            };
        }
    }
}
