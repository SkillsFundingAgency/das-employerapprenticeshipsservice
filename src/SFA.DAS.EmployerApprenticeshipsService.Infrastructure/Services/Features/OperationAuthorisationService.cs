using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class OperationAuthorisationService : IOperationAuthorisationService
    {
        private readonly IFeatureService _featureService;
        private readonly ILog _logger;
        private readonly IOperationAuthorisationHandler[] _pipeSections;

        public OperationAuthorisationService( 
            IFeatureService featureService,
            ILog logger,
            IOperationAuthorisationHandler[] pipeSections)
        {
            _featureService = featureService;
            _logger = logger;
            _pipeSections = pipeSections;
        }


        public bool IsOperationAuthorised(IAuthorizationContext authorisationContext)
        {
            if (authorisationContext.CurrentFeature == null)
            {
                return true;
            }

            var allowedByAllHandlers = _pipeSections.All(handler =>
            {
                var handlerTask = Task.Run(() => handler.CanAccessAsync(authorisationContext)).ConfigureAwait(false);
                return !handlerTask.GetAwaiter().GetResult();
            });

            return allowedByAllHandlers;
        }

        public async Task<bool> CanAccessAsync(IAuthorizationContext authorisationContext)
        {
            if (authorisationContext == null)
            {
                return false;
            }

            try
            {
                foreach (var handler in _pipeSections)
                {
                    if (await handler.CanAccessAsync(authorisationContext) == false)
                    {
                        _logger.Info($"context {authorisationContext.AccountContext?.Id} has been blocked from {authorisationContext.CurrentFeature.FeatureType} by {handler.GetType().Name}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error has occurred when processing a feature toggle pipeline context.");
                throw;
            }
        }
    }
}
