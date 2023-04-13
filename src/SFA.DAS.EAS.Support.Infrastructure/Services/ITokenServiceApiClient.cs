using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public interface ITokenServiceApiClient
{
    Task<PrivilegedAccessToken> GetPrivilegedAccessTokenAsync();
}
