using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class Accounts<T>
    {
        public int AccountsCount { get; set; }
        public List<T> AccountList { get; set; }
    }
}
