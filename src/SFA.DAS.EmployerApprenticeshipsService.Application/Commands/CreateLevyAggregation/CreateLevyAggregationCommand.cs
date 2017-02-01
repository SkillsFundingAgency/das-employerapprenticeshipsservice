using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Commands.CreateLevyAggregation
{
    public class CreateLevyAggregationCommand : IAsyncRequest
    {
        public AggregationData Data { get; set; }
    }
}