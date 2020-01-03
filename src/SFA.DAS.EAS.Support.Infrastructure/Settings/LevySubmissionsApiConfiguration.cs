using SFA.DAS.TokenService.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public class LevySubmissionsApiConfiguration : ITokenServiceApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
       
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string IdentifierUri { get; set; }

        public string Tenant { get; set; }
    }
}
