using System;
using System.Linq;
using NLog;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserView;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class MultiVariantTestingService : AzureServiceBase<UserViewLookup>, IMultiVariantTestingService
    {
        private readonly ICacheProvider _cacheProvider;
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.MultiVariantTesting";
        public sealed override ILogger Logger { get; set; }

        public MultiVariantTestingService(ICacheProvider cacheProvider, ILogger logger)
        {
            _cacheProvider = cacheProvider;
            Logger = logger;
        }

        public UserViewLookup GetUserViews()
        {
            var views = _cacheProvider.Get<UserViewLookup>(nameof(UserViewLookup));

            if (views == null)
            {
                views = GetDataFromStorage();
                if (views.Data != null && views.Data.Any())
                {
                    _cacheProvider.Set(nameof(UserViewLookup),views,new TimeSpan(0,30,0));
                }
            }

            return views;
        }
    }
}