using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IUrlActionHelper
{
    string EmployerAccountsAction(RouteData routeData, string path);
    string EmployerCommitmentsV2Action(RouteData routeData, string path);
    string LevyTransfersMatchingAction(RouteData routeData, string path);
    string ReservationsAction(RouteData routeData, string path);
    string EmployerFinanceAction(RouteData routeData, string path);
    string EmployerIncentivesAction(RouteData routeData, string path = "");
    string EmployerProjectionsAction(RouteData routeData, string path);
    string EmployerRecruitAction(RouteData routeData, string path = "");
    string ProviderRelationshipsAction(RouteData routeData, string path);
    string LegacyEasAccountAction(RouteData routeData, string path);
    string LegacyEasAction(string path);
    string EmployerFeedbackAction(RouteData routeData, string path);
}