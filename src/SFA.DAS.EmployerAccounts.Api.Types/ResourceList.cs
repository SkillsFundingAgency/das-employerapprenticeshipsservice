using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class ResourceList : List<ResourceViewModel>
    {
        public ResourceList(IEnumerable<ResourceViewModel> resources)
        {
            AddRange(resources);
        }
    }
}
