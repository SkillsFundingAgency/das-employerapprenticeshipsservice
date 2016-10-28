using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccounts
{
    public class GetEmployerAccountsQuery : IAsyncRequest<GetEmployerAccountsResponse>
    {
        public string ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
