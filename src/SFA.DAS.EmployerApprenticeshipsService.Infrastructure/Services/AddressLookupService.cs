using System;
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
    public class AddressLookupService : IAddressLookupService
    {
        private const string ParameterAddressIdString = "&key={key}&id={id}";
        private const string ParameterPostCodeOnlyString = "&key={key}&Postcode={postcode}";

        private readonly IRestService _findByPostCodeService;
        private readonly IRestService _findByIdService;
        private readonly PostcodeAnywhereConfiguration _configuration;

        public AddressLookupService(
            IRestServiceFactory restServiceFactory, 
            EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration.PostcodeAnywhere;
            _findByPostCodeService = restServiceFactory.Create(_configuration.FindPartsBaseUrl);
            _findByIdService = restServiceFactory.Create(_configuration.RetrieveServiceBaseUrl);
        }

        public async Task<ICollection<Address>> GetAddressesByPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return null;
            }

            return await Task.Run(() =>
            {
                var restRequest = _findByPostCodeService.Create(
                    ParameterPostCodeOnlyString,
                    new KeyValuePair<string, string>("key", _configuration.Key),
                    new KeyValuePair<string, string>("postcode", postcode));

                restRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                return ValidatedPostalAddresses(restRequest);
            });
        }

        private ICollection<Address> ValidatedPostalAddresses(IRestRequest request)
        {
            
            var result = _findByPostCodeService.Execute<List<PcaServiceFindResult>>(request);

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
            var request = _findByIdService.Create(
                ParameterAddressIdString,
                new KeyValuePair<string, string>("key", _configuration.Key),
                new KeyValuePair<string, string>("id", addressId));

            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var addresses = _findByIdService.Execute<List<PcaAddress>>(request);

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
