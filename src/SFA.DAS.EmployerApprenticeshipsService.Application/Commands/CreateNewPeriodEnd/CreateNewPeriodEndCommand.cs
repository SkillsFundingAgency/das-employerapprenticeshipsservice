using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommand : IAsyncRequest
    {
        public PeriodEnd NewPeriodEnd { get; set; }
    }
}
