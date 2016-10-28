using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
