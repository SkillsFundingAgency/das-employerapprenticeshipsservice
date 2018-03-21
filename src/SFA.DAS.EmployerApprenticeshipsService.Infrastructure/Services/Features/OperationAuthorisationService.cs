using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class OperationAuthorisationService : IOperationAuthorisationService
    {
        private readonly IOperationAuthorisationHandler _operationAuthorisationHandler;

        public OperationAuthorisationService(IOperationAuthorisationHandler operationAuthorisationPipeline)
        {
            _operationAuthorisationHandler = operationAuthorisationPipeline;
        }

        public bool IsOperationAuthorised(string controllerName, string actionName, IAuthorizationContext authorisationContext)
        {
            var request = new OperationContext
            {
                Controller = controllerName,
                Action = actionName,
                AuthorisationContext = authorisationContext
            };

            // Note: this is a blocking operation!
            var x = Task.Run(() => _operationAuthorisationHandler.CanAccessAsync(request)).ConfigureAwait(false);
            return x.GetAwaiter().GetResult();
        }
    }
}
