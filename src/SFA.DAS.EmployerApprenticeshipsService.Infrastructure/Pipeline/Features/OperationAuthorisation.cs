using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Pipeline.Features
{
    public class OperationAuthorisation : IOperationAuthorisationHandler
    {
        private readonly ILog _logger;
        private readonly IOperationAuthorisationHandler[] _pipeSections;

        public OperationAuthorisation(IOperationAuthorisationHandler[] pipeSections, ILog logger)
        {
            _logger = logger;
            _pipeSections = pipeSections;
        }

        public async Task<bool> CanAccessAsync(OperationContext context)
        {
            if (context == null)
            {
                return false;
            }

            try
            {
                foreach (var handler in _pipeSections)
                {
                    if (await handler.CanAccessAsync(context) == false)
                    {
                        _logger.Info($"context {context.MembershipContext?.AccountId} has been blocked from {context.Controller}.{context.Action} by {handler.GetType().Name}");
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
