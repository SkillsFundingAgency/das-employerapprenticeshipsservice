using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class HashingRegistry : Registry
{
    public HashingRegistry()
    {
        For<IHashingService>()
            .Use(c =>
                new HashingService.HashingService(
                    c.GetInstance<EmployerAccountsConfiguration>().AllowedHashstringCharacters,
                    c.GetInstance<EmployerAccountsConfiguration>().Hashstring));

        For<IPublicHashingService>()
            .Use<MarkerInterfaceWrapper>()
            .Ctor<IHashingService>()
            .Is(c =>
                new HashingService.HashingService(
                    c.GetInstance<EmployerAccountsConfiguration>().PublicAllowedHashstringCharacters,
                    c.GetInstance<EmployerAccountsConfiguration>().PublicHashstring)
            );

        For<IAccountLegalEntityPublicHashingService>()
            .Use<MarkerInterfaceWrapper>()
            .Ctor<IHashingService>()
            .Is(c =>
                new HashingService.HashingService(
                    c.GetInstance<EmployerAccountsConfiguration>().PublicAllowedAccountLegalEntityHashstringCharacters,
                    c.GetInstance<EmployerAccountsConfiguration>().PublicAllowedAccountLegalEntityHashstringSalt));

    }
}

public class MarkerInterfaceWrapper
    : IAccountLegalEntityPublicHashingService,
        IPublicHashingService
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
        return _hashingServiceWithCorrectValuesForMarkerInterface.TryDecodeValue(
            input,
            out output);
    }
}