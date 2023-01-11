using SFA.DAS.Caches;
using SFA.DAS.EmployerAccounts.Models.UserView;
using SFA.DAS.NLog.Logger;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.EmployerAccounts.Services;

public class MultiVariantTestingService : AzureServiceBase<MultiVariantViewLookup>, IMultiVariantTestingService
{
    private readonly IInProcessCache _inProcessCache;
    public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.MultiVariantTesting";
    public sealed override ILog Logger { get; set; }

    public MultiVariantTestingService(IInProcessCache inProcessCache, ILog logger, IAutoConfigurationService autoConfigurationService) : base(autoConfigurationService)
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
                _inProcessCache.Set(nameof(MultiVariantViewLookup), views, new TimeSpan(0, 30, 0));
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
        foreach (var view in views.OrderBy(c => c.Weighting))
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