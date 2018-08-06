using MediatR;

namespace SFA.DAS.Queries.GetUserAccounts
{
    public class GetUserAccountsQuery : IAsyncRequest<GetUserAccountsQueryResponse>
    {
        public string UserRef { get; set; }
    }
}
