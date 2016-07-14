using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationWriter : ILevyAggregationWriter
    {
        public void Update(DestinationData data)
        {
            //TODO: Convert to json
            //TODO: Paging [Not Sprint 1]
            //TODO: Write to Azure Table Storage
        }
    }
}