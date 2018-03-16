using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    public class OperationAuthorisationService : IOperationAuthorisationService
    {
        private readonly IOperationAuthorisationHandler _operationAuthorisationHandler;

        public OperationAuthorisationService(IOperationAuthorisationHandler operationAuthorisationPipeline)
        {
            _operationAuthorisationHandler = operationAuthorisationPipeline;
        }

        public bool IsOperationAuthorised(string controllerName, string actionName, IMembershipContext membershipContext)
        {
            var request = new OperationContext
            {
                Controller = controllerName,
                Action = actionName,
                MembershipContext = membershipContext
            };

            return _operationAuthorisationHandler.CanAccessAsync(request).Result;
        }
    }
}
