using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Exceptions;

namespace SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountCoursePayments
{
    public class GetAccountCoursePaymentsQueryHandler : IAsyncRequestHandler<GetAccountCoursePaymentsQuery, GetAccountCoursePaymentsResponse>
    {
        private readonly IValidator<GetAccountCoursePaymentsQuery> _validator;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHmrcDateService _hmrcDateService;
        
        public GetAccountCoursePaymentsQueryHandler(IValidator<GetAccountCoursePaymentsQuery> validator, ITransactionRepository transactionRepository, IHmrcDateService hmrcDateService)
        {
            _validator = validator;
            _transactionRepository = transactionRepository;
            _hmrcDateService = hmrcDateService;
        }

        public async Task<GetAccountCoursePaymentsResponse> Handle(GetAccountCoursePaymentsQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transactions = await _transactionRepository.GetAccountCoursePaymentsByDateRange(
                message.AccountId,
                message.UkPrn,
                message.CourseName,
                message.CourseLevel,
                message.PathwayCode,
                message.FromDate,
                message.ToDate);

            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction.PayrollYear) && transaction.PayrollMonth != 0)
                {
                    transaction.PayrollDate = _hmrcDateService.GetDateFromPayrollYearMonth(transaction.PayrollYear, transaction.PayrollMonth);
                }
            }

            return new GetAccountCoursePaymentsResponse { Transactions = transactions };
        }
    }
}
