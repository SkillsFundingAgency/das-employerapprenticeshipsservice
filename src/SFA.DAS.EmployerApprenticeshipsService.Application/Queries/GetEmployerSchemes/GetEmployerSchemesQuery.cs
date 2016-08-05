using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerSchemes
{
    public class GetEmployerSchemesQuery : IAsyncRequest<GetEmployerSchemesResponse>
    {
        public long Id { get; set; }
    }
}
