using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountResourceList<T> : List<T>, IAccountResource where T : IAccountResource
    {
        public AccountResourceList(IEnumerable<T> resources)
        {
            AddRange(resources);
        }
    }
}
