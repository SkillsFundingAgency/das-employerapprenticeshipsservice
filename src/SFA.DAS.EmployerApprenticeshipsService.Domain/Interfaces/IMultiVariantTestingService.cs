using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.UserView;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IMultiVariantTestingService
    {
        string GetRandomViewNameToShow(List<ViewAccess> views);
    }
}