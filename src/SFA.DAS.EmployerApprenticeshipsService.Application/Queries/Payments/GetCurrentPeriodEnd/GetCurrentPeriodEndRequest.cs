using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.Payments.GetCurrentPeriodEnd
{
    public class GetCurrentPeriodEndRequest : IAsyncRequest<GetPeriodEndResponse>
    {
    }
}
