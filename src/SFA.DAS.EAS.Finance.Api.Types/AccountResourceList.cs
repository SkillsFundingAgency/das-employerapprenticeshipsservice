using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class AccountResourceList<T> : List<T>
    {
        public AccountResourceList(IEnumerable<T> resources)
        {
            AddRange(resources);
        }
    }
}
