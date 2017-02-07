using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdQuery : IAsyncRequest<GetEmployerAccountByHashedIdResponse>
    {
        public string HashedAccountId { get; set; }
    }
}