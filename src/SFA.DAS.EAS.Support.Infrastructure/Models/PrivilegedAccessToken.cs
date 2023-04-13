using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Models;

public class PrivilegedAccessToken
{
    public string AccessCode { get; set; }

    public DateTime ExpiryTime { get; set; }
}
