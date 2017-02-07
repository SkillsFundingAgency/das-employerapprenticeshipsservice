using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetCharity
{
    public class GetCharityQueryRequest : IAsyncRequest<GetCharityQueryResponse>
    {
        public int RegistrationNumber { get; set; }
    }
}
