using SFA.DAS.Authorization.Errors;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class Tier2UserAccesNotGranted : AuthorizationError
    {
        public Tier2UserAccesNotGranted() : base("Tier2 User permission is not granted")
        {

        }
    }
}