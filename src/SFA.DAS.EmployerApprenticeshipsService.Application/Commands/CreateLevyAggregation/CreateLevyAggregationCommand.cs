using MediatR;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Commands.CreateLevyAggregation
{
    public class CreateLevyAggregationCommand : IAsyncRequest
    {
        public AggregationData Data { get; set; }
    }
}