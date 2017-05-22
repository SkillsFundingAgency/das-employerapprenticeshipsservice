using System.Collections.Generic;

using MediatR;

using SFA.DAS.Commitments.Api.Types.ProviderPayment;

namespace SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority
{
    public class UpdateProviderPaymentPriorityCommand : IAsyncRequest
    {
        public long AccountId { get; set; }

        public IEnumerable<ProviderPaymentPriorityItem> Data { get; set; }

        public string UserId { get; set; }

        public string UserEmailAddress { get; set; }

        public string UserDisplayName { get; set; }
    }
}
