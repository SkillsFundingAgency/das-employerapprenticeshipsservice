using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsQuery : IAsyncRequest<GetEmployerAccountTransactionsResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
