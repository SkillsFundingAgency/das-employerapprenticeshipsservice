using MediatR;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommand : IAsyncRequest
    {
        public PeriodEnd NewPeriodEnd { get; set; }
    }
}
