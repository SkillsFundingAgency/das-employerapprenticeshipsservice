using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class AuthorizationContext : IAuthorizationContext
    {
        public IAccountContext AccountContext { get; set; }
        public Feature CurrentFeature { get; set; }
        public IUserContext UserContext { get; set; }
        public IMembershipContext MembershipContext { get; set; }
    };
}