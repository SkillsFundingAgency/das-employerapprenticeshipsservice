using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public int Id { get; set; }
    }
}
