using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web
{
    public static class AddHashingServicesExtensions
    {
        public static void AddHashingServices(this IServiceCollection services, EmployerAccountsConfiguration configuration)
        {
            services.AddTransient<IHashingService>(_ => new HashingService.HashingService(configuration.AllowedHashstringCharacters, configuration.Hashstring));
            services.AddTransient<IPublicHashingService>(_ =>
                new MarkerInterfaceWrapper(
                    new HashingService.HashingService(configuration.PublicAllowedHashstringCharacters, configuration.PublicHashstring)
                    )
            );
            services.AddTransient<IAccountLegalEntityPublicHashingService>(_ =>
                new MarkerInterfaceWrapper(
                    new HashingService.HashingService(configuration.PublicAllowedAccountLegalEntityHashstringCharacters, configuration.PublicAllowedAccountLegalEntityHashstringSalt)
                    )
            );

        }
    }

    internal class MarkerInterfaceWrapper : IAccountLegalEntityPublicHashingService, IPublicHashingService
    {
        private IHashingService _hashingServiceWithCorrectValuesForMarkerInterface;

        public MarkerInterfaceWrapper(IHashingService hashingServiceWithCorrectValuesForMarkerInterface)
        {
            _hashingServiceWithCorrectValuesForMarkerInterface = hashingServiceWithCorrectValuesForMarkerInterface;
        }

        public string HashValue(long id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public string HashValue(Guid id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public string HashValue(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public long DecodeValue(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValue(id);
        }

        public Guid DecodeValueToGuid(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValueToGuid(id);
        }

        public string DecodeValueToString(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValueToString(id);
        }

        public bool TryDecodeValue(string input, out long output)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.TryDecodeValue(input, out output);
        }
    }
}