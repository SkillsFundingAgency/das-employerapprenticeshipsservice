using MediatR;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public Apprenticeship Apprenticeship { get; set; }
    }
}