using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class FeatureToggleService : IFeatureToggleService
    {
        private readonly IPipeline<FeatureToggleRequest, bool> _featurePipeline;
        private readonly ILog _logger;


        public FeatureToggleService(IPipeline<FeatureToggleRequest, bool> featurePipeline, ILog logger)
        {
            _featurePipeline = featurePipeline;
            _logger = logger;
        }

        public bool IsFeatureEnabled(string controllerName, string actionName, IMembershipContext membershipContext)
        {
            var request = new FeatureToggleRequest
            {
                Controller = controllerName,
                Action = actionName,
                MembershipContext = membershipContext
            };


            return _featurePipeline.ProcessAsync(request).Result;
        }
    }
}
