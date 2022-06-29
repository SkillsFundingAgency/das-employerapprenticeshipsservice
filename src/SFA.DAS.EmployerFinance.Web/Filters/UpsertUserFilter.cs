using MediatR;
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
            var mediator = DependencyResolver.Current.GetService<IMediator>();

            var config = DependencyResolver.Current.GetService <EmployerFinanceConfiguration>();
            var constants = new Constants(config.Identity);

            ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            if (identity.Claims.Any())
            {
                var email = identity.Claims.First(c => c.Type == constants.Email()).Value;
                var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;
                var lastName = identity.Claims.First(c => c.Type == constants.FamilyName()).Value;
                var firstName = identity.Claims.First(c => c.Type == constants.GivenName()).Value;

                mediator.SendAsync(new UpsertRegisteredUserCommand
                {
                    EmailAddress = email,
                    UserRef = userRef,
                    LastName = lastName,
                    FirstName = firstName
                }).Wait();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}