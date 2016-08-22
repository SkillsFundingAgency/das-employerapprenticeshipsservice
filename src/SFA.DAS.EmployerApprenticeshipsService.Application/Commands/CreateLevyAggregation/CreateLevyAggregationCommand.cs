using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateLevyAggregation
{
    public class CreateLevyAggregationCommand : IAsyncRequest
    {
        public AggregationData Data { get; set; }
    }
}