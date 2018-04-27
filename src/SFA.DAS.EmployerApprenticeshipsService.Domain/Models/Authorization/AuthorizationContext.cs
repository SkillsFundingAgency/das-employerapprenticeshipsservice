namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class AuthorizationContext : IAuthorizationContext
    {
        public IAccountContext AccountContext { get; set; }
        public IMembershipContext MembershipContext { get; set; }
        public IUserContext UserContext { get; set; }
    };
}