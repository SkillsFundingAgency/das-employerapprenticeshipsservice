using SFA.DAS.EmployerAccounts.Models.UserView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IMultiVariantTestingService
    {
        MultiVariantViewLookup GetMultiVariantViews();
        string GetRandomViewNameToShow(List<ViewAccess> views);
    }
}
