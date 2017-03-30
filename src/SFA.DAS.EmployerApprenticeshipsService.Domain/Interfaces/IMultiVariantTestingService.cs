using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.UserView;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IMultiVariantTestingService
    {
        MultiVariantViewLookup GetMultiVariantViews();

        string GetRandomViewNameToShow(List<ViewAccess> views);
    }
}