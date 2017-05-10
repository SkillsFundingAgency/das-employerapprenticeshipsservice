using MediatR;
using SFA.DAS.EAS.Domain.Models.Payments;


namespace SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommand : IAsyncRequest
    {
        public PeriodEnd NewPeriodEnd { get; set; }
    }
}
