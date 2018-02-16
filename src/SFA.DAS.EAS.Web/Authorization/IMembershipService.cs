using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Web.Authorization
{
    public interface IMembershipService
    {
        IMembershipContext GetMembershipContext();
        void ValidateMembership();
    }
}