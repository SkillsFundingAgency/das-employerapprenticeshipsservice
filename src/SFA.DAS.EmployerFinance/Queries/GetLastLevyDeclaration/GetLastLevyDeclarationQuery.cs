using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationQuery : IAsyncRequest<GetLastLevyDeclarationResponse>
    {
        public string EmpRef { get; set; }
    }
}
