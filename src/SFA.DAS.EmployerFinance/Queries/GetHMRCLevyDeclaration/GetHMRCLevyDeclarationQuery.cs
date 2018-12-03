using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQuery : IAsyncRequest<GetHMRCLevyDeclarationResponse>
    {
        public string EmpRef { get; set; }
    }
}
