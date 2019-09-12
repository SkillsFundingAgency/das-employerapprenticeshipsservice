using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class ResourceList : List<Resource>
    {
        public ResourceList(IEnumerable<Resource> resources)
        {
            AddRange(resources);
        }
    }
}
