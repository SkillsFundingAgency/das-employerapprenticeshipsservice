﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Caches;
using SFA.DAS.Common.Domain.Types;
using DocumentFormat.OpenXml.Wordprocessing;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.ReferenceData.Types.DTO;
using Address = SFA.DAS.EAS.Domain.Models.Organisation.Address;
using Charity = SFA.DAS.EAS.Domain.Models.ReferenceData.Charity;
using OrganisationSubType = SFA.DAS.Common.Domain.Types.OrganisationSubType;
using CommonOrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;
using ReferenceDataOrganisationType = SFA.DAS.ReferenceData.Types.DTO.OrganisationType;
using PublicSectorOrganisation = SFA.DAS.EAS.Domain.Models.ReferenceData.PublicSectorOrganisation;


namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private const int DefaultPageSize = 100;

        private readonly Lazy<Task<CommonOrganisationType[]>> _identifiableOrganisationTypes;
        private readonly IReferenceDataApiClient _client;
        private readonly IMapper _mapper;
        private readonly IInProcessCache _inProcessCache;
        private readonly ILog _logger;

        private readonly List<string> _termsToRemove = new List<string> { "ltd", "ltd.", "limited", "plc", "plc." };

        public ReferenceDataService(
            IReferenceDataApiClient client, 
            IMapper mapper, 
            IInProcessCache inProcessCache,
            ILog logger)
        {
            _client = client;
            _mapper = mapper;
            _inProcessCache = inProcessCache;
            _identifiableOrganisationTypes = new Lazy<Task<CommonOrganisationType[]>>(InitialiseOrganisationTypes);
            _logger = logger;
        }

        public async Task<Charity> GetCharity(int registrationNumber)
        {
            var dto = await _client.GetCharity(registrationNumber);
            var result = _mapper.Map<ReferenceData.Types.DTO.Charity, Charity>(dto);
            return result;
        }

        public Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm)
        {
            return SearchPublicSectorOrganisation(searchTerm, 1, DefaultPageSize);
        }

        public Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber)
        {
            return SearchPublicSectorOrganisation(searchTerm, pageNumber, DefaultPageSize);
        }

        public async Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber, int pageSize)
        {
            var dto = await _client.SearchPublicSectorOrganisation(searchTerm, pageNumber, pageSize);

            var orgainsations = dto.Data.Select(x => _mapper.Map<PublicSectorOrganisation>(x)).ToList();

            return new PagedResponse<PublicSectorOrganisation>
            {
                Data = orgainsations,
                PageNumber = dto.PageNumber,
                TotalPages = dto.TotalPages
            };
        }

        public async Task<PagedResponse<OrganisationName>> SearchOrganisations(string searchTerm, int pageNumber = 1, int pageSize = 25, CommonOrganisationType? organisationType = null)
        {
            var result = await SearchOrganisations(searchTerm);

            if (result == null)
            {
                return new PagedResponse<OrganisationName>();
            }
            
            if (organisationType != null)
            {
                result = FilterOrganisationsByType(result, organisationType.Value);
            }
            
            return CreatePagedOrganisationResponse(pageNumber, pageSize, result);
        }

        public Task<Organisation> GetLatestDetails(CommonOrganisationType organisationType, string identifier)
        {
            return _client.GetLatestDetails(organisationType.ToReferenceDataOrganisationType(), identifier);
        }

        public async Task<bool> IsIdentifiableOrganisationType(CommonOrganisationType organisationType)
        {
            if (organisationType.TryToReferenceDataOrganisationType(out ReferenceDataOrganisationType referenceDataType))
            {
                var locateableOrganisationTypes = await _identifiableOrganisationTypes.Value;

                return locateableOrganisationTypes.Contains(organisationType);
            }

            return false;
        }

        private Task<CommonOrganisationType[]> InitialiseOrganisationTypes()
        {
            return _client.GetIdentifiableOrganisationTypes()
                .ContinueWith(t =>
                        {
                            // switch the organisation types provided by the API back to the organisation type used locally
                            var filteredOrganisationTypes = t.Result
                                .Select(referenceDataOrganisationType => referenceDataOrganisationType.ToCommonOrganisationType())
                                .ToArray();

                            return filteredOrganisationTypes;
                        });
        }

        private List<OrganisationName> SortOrganisations(List<OrganisationName> result, string searchTerm)
        {
            var outputList = new List<OrganisationName>();

            //1. Bob - (exact match - start of the word)
            var priority1RegEx = $"^({searchTerm})$";
            AddResultsMatchingRegEx(result, priority1RegEx, outputList);

            //2. Bob Ltd etc (full word match at the start of the name has company suffix)        
            var priority1ARegEx = $"^({searchTerm}\\W)({string.Join("|", _termsToRemove)})";
            AddResultsMatchingRegEx(result, priority1ARegEx, outputList);

            //2. Bob Hope (full word match at the start of the name)
            var priority2RegEx = $"^({searchTerm}\\W)";
            AddResultsMatchingRegEx(result, priority2RegEx, outputList);

            //3. Bobbing Village School(Matching partial word at the start of a result - alphabetic order)
            //4. Bobby Moore Academy(Matching partial word at the start of a result - alphabetic order)
            //5. Bobby Moore School(Matching partial word at the start of a result - alphabetic order)
            var priority3RegEx = $"^({searchTerm})";
            AddResultsMatchingRegEx(result, priority3RegEx, outputList);

            //6. Ling Bob Nursery School(Matching partial word 6 characters in of a result - alphabetic order)
            //7. Bnos Zion of Bobov(Matching partial word 14 characters in of a result - alphabetic order)
            //8. Talmud Torah Bobov Primary(Matching partial word 14 characters in of a result - alphabetic order)
            AddOrganisationsLooselyMatchingSearchByPosition(result, searchTerm, outputList);

            // Add all the other results back in
            foreach (var item in result)
            {
                if (outputList.Contains(item))
                    continue;

                outputList.Add(item);
            }



            return outputList;
        }

        /// <summary>
        /// Adds any loosely matching organisations, base on the search terms location within the organisation name
        /// </summary>
        /// <param name="rawOrganisations">The list of matching organisations</param>
        /// <param name="searchTerm">The search term used</param>
        /// <param name="outputList">The output list</param>
        private void AddOrganisationsLooselyMatchingSearchByPosition(List<OrganisationName> rawOrganisations, string searchTerm, List<OrganisationName> outputList)
        {
            var priorityRegEx = $"({searchTerm})";

            var rgx = new Regex(priorityRegEx, RegexOptions.IgnoreCase);

            var locationAwareMatches = FindLocationAwareMatches(rawOrganisations, rgx);

            AgregateLocationAwareMatchesToOutList(outputList, locationAwareMatches);
        }

        /// <summary>
        /// For each location aware match, make sure it is added to the output list alphabetically
        /// </summary>
        /// <param name="outputList">The output list</param>
        /// <param name="locationAwareMatches">The location aware mathes to add</param>
        private static void AgregateLocationAwareMatchesToOutList(ICollection<OrganisationName> outputList, IReadOnlyCollection<KeyValuePair<int, OrganisationName>> locationAwareMatches)
        {
            if (locationAwareMatches == null || !locationAwareMatches.Any())
                return;

            foreach (var match in locationAwareMatches.OrderBy(m => m.Key).ThenBy(m => m.Value.Name))
            {
                if (outputList.Contains(match.Value))
                    continue;

                outputList.Add(match.Value);
            }
        }

        private static List<KeyValuePair<int, OrganisationName>> FindLocationAwareMatches(List<OrganisationName> result, Regex rgx)
        {
            var locationAwareMatches = new List<KeyValuePair<int, OrganisationName>>();

            foreach (var item in result)
            {
                var matches = rgx.Matches(item.Name);
                if (matches.Count <= 0)
                    continue;

                locationAwareMatches.Add(new KeyValuePair<int, OrganisationName>(matches[0].Index, item));
            }
            return locationAwareMatches;
        }

        private static void AddResultsMatchingRegEx(List<OrganisationName> result, string priorityRegEx, List<OrganisationName> sortedList)
        {
            var rgx = new Regex(priorityRegEx, RegexOptions.IgnoreCase);

            var priorityItems = result.Where(o => rgx.Matches(o.Name).Count > 0);
            var outList = priorityItems.OrderBy(o => o.Name).ToList();

            foreach (var item in outList)
            {
                if (sortedList.Contains(item))
                    continue;

                sortedList.Add(item);
            }
        }

        private List<OrganisationName> FilterOrganisationsByType(IEnumerable<OrganisationName> result, CommonOrganisationType organisationType)
        {
            if (organisationType == CommonOrganisationType.Other || organisationType == CommonOrganisationType.PublicBodies)
            {
                return result.Where(x => x.Type == CommonOrganisationType.Other || x.Type == CommonOrganisationType.PublicBodies).ToList();
            }
            return result.Where(x => x.Type == organisationType).ToList();
        }

        private async Task<List<OrganisationName>> SearchOrganisations(string searchTerm)
        {
            var cacheKey = $"SearchKey_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(searchTerm))}";

            var result = _inProcessCache.Get<List<OrganisationName>>(cacheKey);
            if (result != null) return result;

            var orgs = await _client.SearchOrganisations(searchTerm);

            if (orgs == null) return new List<OrganisationName>();

            var convertedOrgs = orgs.Select(ConvertToOrganisation).ToList();

            result = SortOrganisations(convertedOrgs, searchTerm);

            _inProcessCache.Set(cacheKey, result, new TimeSpan(0, 15, 0));
            return result;
        }

        private static PagedResponse<OrganisationName> CreatePagedOrganisationResponse(int pageNumber, int pageSize, List<OrganisationName> result)
        {
            return new PagedResponse<OrganisationName>
            {
                Data = result.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList(),
                TotalPages = (int)Math.Ceiling(((decimal) result.Count / pageSize)),
                PageNumber = pageNumber,
                TotalResults = result.Count
            };
        }

        private OrganisationName ConvertToOrganisation(Organisation source)
        {
            return new OrganisationName
            {
                Address = new Address
                {
                    Line1 = source.Address.Line1,
                    Line2 = source.Address.Line2,
                    Line3 = source.Address.Line3,
                    Line4 = source.Address.Line4,
                    Line5 = source.Address.Line5,
                    Postcode = source.Address.Postcode
                },
                Name = source.Name,
                Code = source.Code,
                RegistrationDate = source.RegistrationDate,
                Sector = source.Sector,
                SubType = ConvertToOrganisationSubType(source.SubType),
                Type = source.Type.ToCommonOrganisationType()
            };
        }

        private OrganisationSubType ConvertToOrganisationSubType(ReferenceData.Types.DTO.OrganisationSubType sourceSubType)
        {
            switch (sourceSubType)
            {
                case ReferenceData.Types.DTO.OrganisationSubType.Nhs:
                    return OrganisationSubType.Nhs;
                case ReferenceData.Types.DTO.OrganisationSubType.Ons:
                    return OrganisationSubType.Ons;
                case ReferenceData.Types.DTO.OrganisationSubType.Police:
                    return OrganisationSubType.Police;
                default:
                    return 0;
            }
        }
    }
}
