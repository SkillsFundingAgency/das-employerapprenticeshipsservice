using System;
using MediatR;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;

namespace SFA.DAS.EAS.Application.Commands.UpdateApprenticeshipStatus
{
    public sealed class UpdateApprenticeshipStatusCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public long EmployerAccountId { get; set; }
        public string UserId { get; set; }
        public ChangeStatusType ChangeType { get; set; }
        public DateTime DateOfChange { get; set; }
    }
}
