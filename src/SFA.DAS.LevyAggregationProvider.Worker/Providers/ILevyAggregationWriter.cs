using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public interface ILevyAggregationWriter
    {
        void Update(DestinationData data);
    }
}