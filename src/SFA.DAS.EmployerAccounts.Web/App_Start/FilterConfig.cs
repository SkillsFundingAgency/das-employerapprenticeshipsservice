using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.Authorization.Mvc.Filters;
using System;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using System.Net;
using System.Web.Routing;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.Web;

public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new AnalyticsFilter());
    }
}