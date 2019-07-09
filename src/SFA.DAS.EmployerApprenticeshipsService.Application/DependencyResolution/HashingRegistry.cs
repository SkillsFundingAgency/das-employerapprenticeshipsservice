using System;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IHashingService>()
                .Use(
                    c =>
                        new HashingService.HashingService(
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .AllowedHashstringCharacters,
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .Hashstring));

            For<MarkerInterfaces.IPublicHashingService>()
                .Use<MarkerInterfaceWrapper>()
                .Ctor<IHashingService>()
                .Is(
                    c =>
                        new HashingService.HashingService(
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicAllowedHashstringCharacters,
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicHashstring)
                );

            For<Infrastructure.MarkerInterfaces.IAccountLegalEntityPublicHashingService>()
                .Add<MarkerInterfaceWrapper>()
                .Ctor<IHashingService>()
                .Is(
                    c =>
                        new HashingService.HashingService(
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicAllowedAccountLegalEntityHashstringCharacters,
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicAllowedAccountLegalEntityHashstringSalt));
        }
    }

    class MarkerInterfaceWrapper
        : Infrastructure.MarkerInterfaces.IAccountLegalEntityPublicHashingService,
            MarkerInterfaces.IPublicHashingService
    {
        private readonly IHashingService
            _hashingServiceWithCorrectValuesForMarkerInterface;

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
}