namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IAuthorizationContext
    {
        IAccountContext AccountContext { get; }
        IMembershipContext MembershipContext { get; }
        IUserContext UserContext { get; }
    }
}