using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQuery : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public string Id { get; set; }
    }
}
