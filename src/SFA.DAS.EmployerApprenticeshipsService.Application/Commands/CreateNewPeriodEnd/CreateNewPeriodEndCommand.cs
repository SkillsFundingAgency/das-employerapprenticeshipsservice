using MediatR;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommand : IAsyncRequest
    {
        public PeriodEnd NewPeriodEnd { get; set; }
    }
}
