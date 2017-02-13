using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationRequest : IAsyncRequest<GetLastLevyDeclarationResponse>
    {
        public string Empref { get; set; }
    }
}
