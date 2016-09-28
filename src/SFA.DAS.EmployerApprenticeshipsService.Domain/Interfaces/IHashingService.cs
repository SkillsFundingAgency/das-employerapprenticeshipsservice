using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IHashingService
    {
        string HashValue(long id);
        long DecodeValue(string id);
    }
}
