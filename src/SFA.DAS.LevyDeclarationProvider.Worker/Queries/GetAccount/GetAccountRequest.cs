using MediatR;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Queries.GetAccount
{
    public class GetAccountRequest : IAsyncRequest<GetAccountResponse>
    {
        public long AccountId { get; set; }
    }
}