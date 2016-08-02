using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationHandler : IAsyncRequestHandler<GetHmrcEmployerInformatioQuery, GetHmrcEmployerInformatioResponse>
    {
        public Task<GetHmrcEmployerInformatioResponse> Handle(GetHmrcEmployerInformatioQuery message)
        {
            throw new NotImplementedException();
        }
    }
}
