using MediatR;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;
using System;

namespace SFA.DAS.EAS.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQuery : IAsyncRequest<ValidateStatusChangeDateQueryResponse>
    {
        public long AccountId { get; set; }

        public long ApprenticeshipId { get; set; }

        public DateTime? DateOfChange { get; set; }

        public ChangeOption ChangeOption { get; set; }
    }
}
