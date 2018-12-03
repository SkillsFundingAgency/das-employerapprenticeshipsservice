using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public interface IAccountViewModel
    {
        long AccountId { get; set; }
        string AccountHashedId { get; set; }
    }
}
