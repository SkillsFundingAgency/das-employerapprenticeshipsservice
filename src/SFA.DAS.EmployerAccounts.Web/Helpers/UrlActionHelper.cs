using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.EmployerAccounts.Web.Helpers;

public class UrlActionHelper : IUrlActionHelper
{
    private readonly EmployerAccountsConfiguration _configuration;

    public UrlActionHelper(EmployerAccountsConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string EmployerAccountsAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerAccountsBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string EmployerCommitmentsV2Action(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerCommitmentsV2BaseUrl;

        return NonAccountsAction(routeData, baseUrl, path);
    }

    public string LevyTransfersMatchingAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.LevyTransferMatchingBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string ReservationsAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.ReservationsBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string EmployerFinanceAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerFinanceBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string EmployerIncentivesAction(RouteData routeData, string path = "")
    {
        var baseUrl = _configuration.EmployerIncentivesBaseUrl;
        var hashedAccountId = routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
        return Action(baseUrl, $"{hashedAccountId}/{path}");
    }

    public string EmployerProjectionsAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerProjectionsBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string EmployerRecruitAction(RouteData routeData, string path = "")
    {
        var baseUrl = _configuration.EmployerRecruitBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string ProviderRelationshipsAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.ProviderRelationshipsBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string LegacyEasAccountAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerPortalBaseUrl;

        return AccountAction(routeData, baseUrl, path);
    }

    public string LegacyEasAction(string path)
    {
        var baseUrl = _configuration.EmployerPortalBaseUrl;

        return Action(baseUrl, path);
    }

    private static string AccountAction(RouteData routeData, string baseUrl, string path)
    {
        var hashedAccountId = routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
        var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

        return Action(baseUrl, accountPath);
    }

    // unlike the rest of the services within MA - commitments v2 does not have 'accounts/' in its urls
    // Nor does Employer Feedback
    private static string NonAccountsAction(RouteData routeData, string baseUrl, string path)
    {

        var hashedAccountId = routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
        var commitmentPath = hashedAccountId == null ? $"{path}" : $"{hashedAccountId}/{path}";

        return Action(baseUrl, commitmentPath);
    }

    public string EmployerFeedbackAction(RouteData routeData, string path)
    {
        var baseUrl = _configuration.EmployerFeedbackBaseUrl;

        return NonAccountsAction(routeData, baseUrl, path);
    }

    private static string Action(string baseUrl, string path)
    {
        var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

        return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
    }
}