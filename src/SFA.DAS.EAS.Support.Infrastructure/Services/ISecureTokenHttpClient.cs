using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

internal interface ISecureTokenHttpClient
{
    Task<string> GetAsync(string url);
}
