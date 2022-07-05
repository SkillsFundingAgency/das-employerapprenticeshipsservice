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
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                if (session?[UpsertUserRequired] as bool? ?? false)
                {
                    Logger.Info("UpsertUserFilter: Retrieving claims after authentication");

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

                            Logger.Info($"UpsertUserFilter: Retrieved claims after authentication email={email}, userRef={userRef}, lastName={lastName}, firstName={firstName}");

                            var mediator = DependencyResolver.Current.GetService<IMediator>();
                            mediator.Send(new UpsertRegisteredUserCommand
                            {
                                EmailAddress = email,
                                UserRef = userRef,
                                LastName = lastName,
                                FirstName = firstName
                            });

                            Logger.Info($"UpsertUserFilter: Upserted user with claims after authentication email={email}, userRef={userRef}, lastName={lastName}, firstName={firstName}");
                            session[UpsertUserRequired] = false;
                        }
                        else
                        {
                            Logger.Info("UpsertUserFilter: Unable to retrieve claims after authentication");
                        }
                    }
                }
                else
                {
                    Logger.Info("UpsertUserFilter: Upsert user is not required");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpsertUserFilter: Error unable to upsert user with identity claims");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}