using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class ResourceList : List<ResourceViewModel>
    {
        public ResourceList(IEnumerable<ResourceViewModel> resources)
        {
            AddRange(resources);
        }
    }
}
