using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Employer;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;
using SFA.DAS.EAS.Infrastructure.Models;
using SFA.DAS.EAS.Infrastructure.Models.PostcodeAnywhere;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class AddressLookupService : RestService, IAddressLookupService
    {
        private const string ParameterAddressIdString = "&key={key}&id={id}";
        private const string ParameterPostCodeOnlyString = "&key={key}&Postcode={postcode}";

        private readonly PostcodeAnywhereConfiguration _configuration;

        public AddressLookupService(IRestClientFactory restClientFactory, EmployerApprenticeshipsServiceConfiguration configuration)
            :base(restClientFactory)
        {
            _configuration = configuration.PostcodeAnywhere;
        }

        public async Task<ICollection<Address>> GetAddressesByPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return null;
            }

            return await Task.Run(() =>
            {
                var restRequest = Create(
                    ParameterPostCodeOnlyString,
                    new KeyValuePair<string, string>("key", _configuration.Key),
                    new KeyValuePair<string, string>("postcode", postcode));

                restRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                return ValidatedPostalAddresses(restRequest);
            });
        }

        private ICollection<Address> ValidatedPostalAddresses(IRestRequest request)
        {
            var result = Execute<List<PcaServiceFindResult>>(request);

            var addresses = result.Data?.Where(x => x.Id != null).ToArray();

            if (addresses == null || !addresses.Any())
            {
                return null;
            }

            var validatedAddresses = addresses.Select(x => RetrieveValidatedAddress(x.Id))
                                              .Where(x => x != null)
                                              .ToList();

            return validatedAddresses;
        }

        private Address RetrieveValidatedAddress(string addressId)
        {
            var request = Create(
                ParameterAddressIdString,
                new KeyValuePair<string, string>("key", _configuration.Key),
                new KeyValuePair<string, string>("id", addressId));

            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var addresses = Execute<List<PcaAddress>>(request);

            var address = addresses.Data?.SingleOrDefault();

            if (address?.Udprn == null)
            {
                return null;
            }

            var result = new Address
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                TownOrCity = address.PostTown,
                County = address.County,
                PostCode = address.Postcode
            };

            return result;
        }
    }
}
