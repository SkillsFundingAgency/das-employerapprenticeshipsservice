using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class ResourceList : List<ResourceViewModel>
    {
        public ResourceList(IEnumerable<ResourceViewModel> resources)
        {
            AddRange(resources);
        }
    }
}
