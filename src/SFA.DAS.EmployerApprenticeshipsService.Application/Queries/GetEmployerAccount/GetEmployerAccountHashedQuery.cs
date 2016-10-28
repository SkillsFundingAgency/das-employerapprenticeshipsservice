using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHashedQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedId { get; set; }
        public string UserId { get; set; }
    }
}
