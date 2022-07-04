using MediatR;
using NLog;
using SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser;
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
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                if (session?.IsNewSession ?? false)
                {
                    Logger.Info("UpsertUserFilter: Retrieving claims for new user session");

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

                            Logger.Info($"UpsertUserFilter: Retrieved claims for new user session email={email}, userRef={userRef}, lastName={lastName}, firstName={firstName}");

                            var mediator = DependencyResolver.Current.GetService<IMediator>();
                            mediator.Send(new UpsertRegisteredUserCommand
                            {
                                EmailAddress = email,
                                UserRef = userRef,
                                LastName = lastName,
                                FirstName = firstName
                            });
                        }
                        else
                        {
                            Logger.Info("UpsertUserFilter: Unable to retrieve claims for new user session");
                        }
                    }
                }
                else
                {
                    Logger.Info("UpsertUserFilter: Not retrieving claims for existing session");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpsertUserFilter: Error unable synchronize user with claims for session");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}