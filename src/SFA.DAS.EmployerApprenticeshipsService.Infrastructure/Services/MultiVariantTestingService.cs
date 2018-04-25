using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserView;
using SFA.DAS.EAS.Infrastructure.Caching;
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

        public MultiVariantViewLookup GetMultiVariantViews()
        {
            var views = _inProcessCache.Get<MultiVariantViewLookup>(nameof(MultiVariantViewLookup));

            if (views == null)
            {
                views = GetDataFromTableStorage();
                if (views.Data != null && views.Data.Any())
                {
                    _inProcessCache.Set(nameof(MultiVariantViewLookup),views,new TimeSpan(0,30,0));
                }
            }

            return views;
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