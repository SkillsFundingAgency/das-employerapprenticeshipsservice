using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerFinance.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class UpsertUserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();

            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session.IsNewSession)
            {
                logger.LogInformation("UpsertUserFilter: A new session");

                //var mediator = DependencyResolver.Current.GetService<IMediator>();

                var config = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
                var constants = new Constants(config.Identity);

                ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (identity.Claims.Any())
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

                    logger.LogInformation($"UpsertUserFilter: claims {email}, {userRef}, {lastName}, {firstName}");
                }
                else
                {
                    logger.LogInformation("UpsertUserFilter: No claims");
                }
            }
            else
            {
                logger.LogInformation("UpsertUserFilter: Not a new session");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}