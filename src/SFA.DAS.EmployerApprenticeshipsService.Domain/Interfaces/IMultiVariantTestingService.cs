using SFA.DAS.EAS.Domain.Models.UserView;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IMultiVariantTestingService
    {
        UserViewLookup GetUserViews();
    }
}