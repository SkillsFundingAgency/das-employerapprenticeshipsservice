using SFA.DAS.EAS.Portal.Extensions;

namespace SFA.DAS.EAS.Portal.Configuration
{
    public class PortalConfiguration
    {
        public string ServiceBusConnectionString { get; set; }

        public string NServiceBusLicense
        {
            get => _decodedNServiceBusLicense ?? (_decodedNServiceBusLicense = _nServiceBusLicense.HtmlDecode());
            set => _nServiceBusLicense = value;
        }

        private string _nServiceBusLicense;
        private string _decodedNServiceBusLicense;
    }
}
