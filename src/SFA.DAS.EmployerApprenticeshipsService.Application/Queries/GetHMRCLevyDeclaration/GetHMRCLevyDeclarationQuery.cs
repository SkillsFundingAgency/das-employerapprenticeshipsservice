using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQuery : IAsyncRequest<GetHMRCLevyDeclarationResponse>
    {
        public string Id { get; set; }
    }
}
