using System;

namespace SFA.DAS.EAS.Support.Infrastructure.Models;

public class PrivilegedAccessToken
{
    public string AccessCode { get; set; }

    public DateTime ExpiryTime { get; set; }
}
