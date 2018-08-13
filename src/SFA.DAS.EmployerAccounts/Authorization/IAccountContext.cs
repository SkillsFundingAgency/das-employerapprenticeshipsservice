using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IAccountContext
    {
        long Id { get; }
        string HashedId { get; }
        string PublicHashedId { get; }
    }
}
