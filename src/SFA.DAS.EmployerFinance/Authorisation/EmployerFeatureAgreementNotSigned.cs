using SFA.DAS.Authorization.Errors;

namespace SFA.DAS.EmployerFinance.Authorisation
{
    public class EmployerFeatureAgreementNotSigned : AuthorizationError
    {
        public EmployerFeatureAgreementNotSigned() : base("Employer feature agreement not signed")
        {
        }
    }
}
