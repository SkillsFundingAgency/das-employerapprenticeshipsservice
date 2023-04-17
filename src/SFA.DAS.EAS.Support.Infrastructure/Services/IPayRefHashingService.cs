using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public interface IPayRefHashingService
{
    string HashValue(string id);
    string DecodeValueToString(string id);
}
