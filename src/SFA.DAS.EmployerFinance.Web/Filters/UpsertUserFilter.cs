using NLog;
using SFA.DAS.EmployerFinance.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class UpsertUserFilter : ActionFilterAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Logger.Info("UpsertUserFilter: getting session");

                HttpSessionStateBase session = filterContext.HttpContext.Session;
                if (session == null)
                {
                    Logger.Info("UpsertUserFilter: Session is null");
                }
                else if (session.IsNewSession)
                {
                    Logger.Info("UpsertUserFilter: A new session");

                    //var mediator = DependencyResolver.Current.GetService<IMediator>();

                    var config = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
                    if (config != null)
                    {


                        var constants = new Constants(config.Identity);

                        ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                        if (identity?.Claims?.Any() ?? false)
                        {
                            var email = identity.Claims.FirstOrDefault(c => c.Type == constants.Email()).Value;
                            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;
                            var lastName = identity.Claims.FirstOrDefault(c => c.Type == constants.FamilyName()).Value;
                            var firstName = identity.Claims.FirstOrDefault(c => c.Type == constants.GivenName()).Value;

                            /*mediator.SendAsync(new UpsertRegisteredUserCommand
                            {
                                EmailAddress = email,
                                UserRef = userRef,
                                LastName = lastName,
                                FirstName = firstName
                            }).Wait();*/

                            Logger.Info($"UpsertUserFilter: claims {email}, {userRef}, {lastName}, {firstName}");
                        }
                        else
                        {
                            Logger.Info("UpsertUserFilter: No claims");
                        }
                    }
                    else
                    {
                        Logger.Info("UpsertUserFilter: No config");
                    }
                }
                else
                {
                    Logger.Info("UpsertUserFilter: Not a new session");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpsertUserFilter: error");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}