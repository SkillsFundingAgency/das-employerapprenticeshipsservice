using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models
{
    public class AccountResourceList<T> : List<T>, IAccountResource where T : IAccountResource
    {
        public AccountResourceList(IEnumerable<T> resources)
        {
            AddRange(resources);
        }
    }
}
