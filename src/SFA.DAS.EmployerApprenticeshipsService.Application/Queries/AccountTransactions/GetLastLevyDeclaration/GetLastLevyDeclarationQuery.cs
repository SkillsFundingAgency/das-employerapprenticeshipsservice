using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationQuery : IAsyncRequest<GetLastLevyDeclarationResponse>
    {
        public string EmpRef { get; set; }
    }
}
