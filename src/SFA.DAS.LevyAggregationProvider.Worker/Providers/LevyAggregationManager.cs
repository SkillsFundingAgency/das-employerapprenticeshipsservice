using System;
using System.Threading.Tasks;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationManager
    {
        private readonly ILevyDeclarationReader _levyDeclarationReader;
        private readonly ILevyAggregationWriter _levyAggregationWriter;

        public LevyAggregationManager(ILevyDeclarationReader levyDeclarationReader, ILevyAggregationWriter levyAggregationWriter)
        {
            if (levyDeclarationReader == null)
                throw new ArgumentNullException(nameof(levyDeclarationReader));
            if (levyAggregationWriter == null)
                throw new ArgumentNullException(nameof(levyAggregationWriter));
            _levyDeclarationReader = levyDeclarationReader;
            _levyAggregationWriter = levyAggregationWriter;
        }

        public async Task Process(string empRef)
        {
            var sourceData = await _levyDeclarationReader.GetData(empRef);

            var aggregator = new LevyAggregator();

            var destinationData = aggregator.BuildAggregate(sourceData);

            _levyAggregationWriter.Update(destinationData);
        }
    }
}