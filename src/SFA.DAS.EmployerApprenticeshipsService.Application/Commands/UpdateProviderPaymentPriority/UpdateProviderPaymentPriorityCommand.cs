using System.Collections.Generic;

using MediatR;

using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;

namespace SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority
{
    public class UpdateProviderPaymentPriorityCommand : IAsyncRequest
    {
        public long AccountId { get; set; }

        public IEnumerable<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> Data { get; set; }

    }
}
