using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationRequest : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public long AccountId { get; set; }
    }
}