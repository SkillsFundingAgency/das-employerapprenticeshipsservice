using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQuery : IAsyncRequest<GetUserAccountsQueryResponse>
    {
        public string UserId { get; set; }
    }
}
