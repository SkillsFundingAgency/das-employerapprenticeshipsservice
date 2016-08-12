using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetPayeSchemeInUse
{
    public class GetPayeSchemeInUseQuery : IAsyncRequest<GetPayeSchemeInUseResponse>
    {
        public string Empref { get; set; }
    }
}
