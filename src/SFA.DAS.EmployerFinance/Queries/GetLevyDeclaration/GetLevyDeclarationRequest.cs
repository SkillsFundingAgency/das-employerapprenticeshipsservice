using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationRequest : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public string HashedAccountId { get; set; }
    }
}
