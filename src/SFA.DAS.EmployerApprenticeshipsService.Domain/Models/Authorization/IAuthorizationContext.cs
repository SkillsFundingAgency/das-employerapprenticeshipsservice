namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IAuthorizationContext
    {
        IAccountContext AccountContext { get; }
        IUserContext UserContext { get; }
        IMembershipContext MembershipContext { get; }
    }
}