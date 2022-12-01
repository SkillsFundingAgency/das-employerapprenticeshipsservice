using MediatR;
using SFA.DAS.EmployerFinance.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTotalPayments
{
    public class GetTotalPaymentsQueryHandler
        : IAsyncRequestHandler<GetTotalPaymentsQuery, GetTotalPaymentsResponse>
    {
        private readonly EmployerFinanceDbContext _financeDb;

        public GetTotalPaymentsQueryHandler(EmployerFinanceDbContext financeDb)
        {
            _financeDb = financeDb;
        }

        public async Task<GetTotalPaymentsResponse> Handle(GetTotalPaymentsQuery message)
        {
            return new GetTotalPaymentsResponse
            {
                TotalPayments = await _financeDb.Payments.CountAsync()
            };
        }
    }
}
