using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken
{
    public class GetGatewayTokenQuery : IAsyncRequest<GetGatewayTokenQueryResponse>
    {
        public string AccessCode { get; set; }
        public string RedirectUrl { get; set; }
    }
}
