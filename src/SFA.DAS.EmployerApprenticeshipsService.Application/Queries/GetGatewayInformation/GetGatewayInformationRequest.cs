using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation
{
    public class GetGatewayInformationQuery : IAsyncRequest<GetGatewayInformationResponse>
    {
        public string ReturnUrl { get; set; }
    }
}
