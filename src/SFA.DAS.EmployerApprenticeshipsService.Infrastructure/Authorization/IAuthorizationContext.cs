namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAuthorizationContext
    {
        IAccountContext AccountContext { get; }
        IMembershipContext MembershipContext { get; }
        IUserContext UserContext { get; }
    }
}