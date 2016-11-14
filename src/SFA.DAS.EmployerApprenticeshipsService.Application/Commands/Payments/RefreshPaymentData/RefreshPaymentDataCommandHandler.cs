using System;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
{
    public class RefreshPaymentDataCommandHandler : AsyncRequestHandler<RefreshPaymentDataCommand>
    {
        protected override Task HandleCore(RefreshPaymentDataCommand message)
        {
            throw new NotImplementedException();
        }
    }
}