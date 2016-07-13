using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregator
    {
        public DestinationData BuildAggregate(SourceData input)
        {
            //Build aggregate structures
            //Convert to json

            return new DestinationData
            {
                AccountId = input.AccountId
            };
        }
    }
}