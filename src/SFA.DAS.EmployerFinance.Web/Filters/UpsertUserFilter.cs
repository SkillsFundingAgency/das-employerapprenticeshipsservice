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
        public const string UpsertUserRequired = nameof(UpsertUserRequired);
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Logger.Info("UpsertUserFilter: Retrieving claims");

                var config = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
                if (config != null)
                {
                    ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                    if (identity?.Claims?.Any() ?? false)
                    {
                        var constants = new Constants(config.Identity);
                        var email = identity.Claims.FirstOrDefault(c => c.Type == constants.Email()).Value;
                        var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;
                        var lastName = identity.Claims.FirstOrDefault(c => c.Type == constants.FamilyName()).Value;
                        var firstName = identity.Claims.FirstOrDefault(c => c.Type == constants.GivenName()).Value;

                        Logger.Info($"UpsertUserFilter: Retrieved claims email={email}, userRef={userRef}, lastName={lastName}, firstName={firstName}");

                        var mediator = DependencyResolver.Current.GetService<IMediator>();
                        mediator.Send(new UpsertRegisteredUserCommand
                        {
                            EmailAddress = email,
                            UserRef = userRef,
                            LastName = lastName,
                            FirstName = firstName
                        });

                        Logger.Info($"UpsertUserFilter: Upserted user with claims email={email}, userRef={userRef}, lastName={lastName}, firstName={firstName}");
                    }
                    else
                    {
                        Logger.Info("UpsertUserFilter: Unable to retrieve claims");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpsertUserFilter: Unable to upsert user with claims");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}