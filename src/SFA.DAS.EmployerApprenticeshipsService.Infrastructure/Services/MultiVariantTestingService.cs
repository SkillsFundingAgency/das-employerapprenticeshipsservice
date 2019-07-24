using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Caches;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserView;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class MultiVariantTestingService : AzureServiceBase<MultiVariantViewLookup>, IMultiVariantTestingService
    {
        private readonly IInProcessCache _inProcessCache;
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.MultiVariantTesting";
        public sealed override ILog Logger { get; set; }

        public MultiVariantTestingService(IInProcessCache inProcessCache, ILog logger)
        {
            _inProcessCache = inProcessCache;
            Logger = logger;
        }

        public string GetRandomViewNameToShow(List<ViewAccess> views)
        {
            if (views.Count == 1)
            {
                return null;
            }

            var maxValue = 11;
            var randomNumber = new Random().Next(maxValue);

            var viewName = string.Empty;
            foreach (var view in views.OrderBy(c=>c.Weighting))
            {
                if (randomNumber >= (maxValue - view.Weighting) && randomNumber < maxValue)
                {
                    viewName = view.ViewName;
                    continue;
                }

                maxValue = maxValue - view.Weighting;
            }

            return viewName;
        }
        
    }
}