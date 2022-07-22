using System.Collections.Generic;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class AccountResourceList<T> : List<T>
    {
        public AccountResourceList(IEnumerable<T> resources)
        {
            AddRange(resources);
        }
    }
}
