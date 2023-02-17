using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;
using SFA.DAS.EmployerAccounts.Models.UserView;

namespace SFA.DAS.EmployerAccounts.Services;

public class MultiVariantTestingService : AzureServiceBase<MultiVariantViewLookup, MultiVariantTestingService>, IMultiVariantTestingService
{
    private const int CacheExpirationMinutes = 30;
    private readonly IInProcessCache _inProcessCache;
    public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.MultiVariantTesting";
    public sealed override ILogger<MultiVariantTestingService> Logger { get; set; }

    public MultiVariantTestingService(
        IInProcessCache inProcessCache, 
        ILogger<MultiVariantTestingService> logger, 
        IAutoConfigurationService autoConfigurationService, 
        IConfiguration configuration) 
        : base(autoConfigurationService, configuration)
    {
        _inProcessCache = inProcessCache;
        Logger = logger;
    }

    public MultiVariantViewLookup GetMultiVariantViews()
    {
        var views = _inProcessCache.Get<MultiVariantViewLookup>(nameof(MultiVariantViewLookup));

        if (views != null)
        {
            return views;
        }

        views = GetDataFromTableStorage();

        if (views.Data != null && views.Data.Any())
        {
            _inProcessCache.Set(nameof(MultiVariantViewLookup), views, new TimeSpan(0, CacheExpirationMinutes, 0));
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
        foreach (var view in views.OrderBy(c => c.Weighting))
        {
            if (randomNumber >= (maxValue - view.Weighting) && randomNumber < maxValue)
            {
                viewName = view.ViewName;
                continue;
            }

            maxValue -= view.Weighting;
        }

        return viewName;
    }
}