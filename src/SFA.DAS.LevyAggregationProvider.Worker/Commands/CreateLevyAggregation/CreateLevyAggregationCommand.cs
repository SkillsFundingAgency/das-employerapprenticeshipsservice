using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.LevyAggregationProvider.Worker.Commands.CreateLevyAggregation
{
    public class CreateLevyAggregationCommand : IAsyncRequest
    {
        public AggregationData Data { get; set; }
    }
}