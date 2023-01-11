using SFA.DAS.EmployerAccounts.Models.UserView;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IMultiVariantTestingService
{
    MultiVariantViewLookup GetMultiVariantViews();
    string GetRandomViewNameToShow(List<ViewAccess> views);
}