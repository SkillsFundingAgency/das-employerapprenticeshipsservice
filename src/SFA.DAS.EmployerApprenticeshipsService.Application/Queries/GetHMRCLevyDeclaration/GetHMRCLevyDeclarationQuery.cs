using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQuery : IAsyncRequest<GetHMRCLevyDeclarationResponse>
    {
        public string AuthToken { get; set; }

        public string EmpRef { get; set; }
    }
}
