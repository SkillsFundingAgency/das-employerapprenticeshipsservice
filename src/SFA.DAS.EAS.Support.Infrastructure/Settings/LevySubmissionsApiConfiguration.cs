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

        public string LevyTokenCertificatethumprint { get; set; }

        public X509Certificate TokenCertificate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LevyTokenCertificatethumprint))
                {
                    return null;
                }

                var store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                try
                {

                    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, LevyTokenCertificatethumprint, false);

                    if (certificates.Count > 0)
                    {
                        return certificates[0];
                    }
                }
                finally
                {
                    store.Close();
                }

                throw new KeyNotFoundException($"Couldn't find the certificate for thumbprint '{LevyTokenCertificatethumprint}'");
            }
            set { }
        }


    }
}
