using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features
{
    public class FeatureTogglePipeline : IPipeline<FeatureToggleRequest, bool>
    {
        private readonly ILog _logger;
        private readonly IEnumerable<IPipelineSection<FeatureToggleRequest, bool>> _orderedPipeSections;

        public FeatureTogglePipeline(IEnumerable<IPipelineSection<FeatureToggleRequest, bool>> pipeSections, ILog logger)
        {
            _logger = logger;
            _orderedPipeSections = pipeSections?.OrderBy(x => x.Priority);
        }

        public async Task<bool> ProcessAsync(FeatureToggleRequest request)
        {
            try
            {
                foreach (var section in _orderedPipeSections)
                {
                    var result = await section.ProcessAsync(request).ConfigureAwait(false);

                    if (!result)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error has occurred when processing a feature toggle pipeline request.");
                throw;
            }
        }
    }
}
