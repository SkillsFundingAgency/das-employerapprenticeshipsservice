using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class Accounts<T>
    {
        public int AccountsCount { get; set; }
        public List<T> AccountList { get; set; }
    }
}
