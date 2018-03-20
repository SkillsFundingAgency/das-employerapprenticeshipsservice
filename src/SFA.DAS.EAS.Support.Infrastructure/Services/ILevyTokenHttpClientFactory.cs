using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.TokenService.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface ILevyTokenHttpClientFactory
    {
        Task<IApprenticeshipLevyApiClient> GetLevyHttpClient();
    }
}
