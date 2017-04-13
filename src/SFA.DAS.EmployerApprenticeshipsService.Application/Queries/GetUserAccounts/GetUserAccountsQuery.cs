using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQuery : IAsyncRequest<GetUserAccountsQueryResponse>
    {
        public string UserRef { get; set; }
    }
}
