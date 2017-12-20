using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.ReferenceData.Api.Client;


namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private const int DefaultPageSize = 100;

        private readonly IReferenceDataApiClient _client;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;

        private readonly List<string> _termsToRemove = new List<string> { "ltd", "ltd.", "limited", "plc", "plc." };

        public ReferenceDataService(IReferenceDataApiClient client, IMapper mapper, ICacheProvider cacheProvider)
        {
            _client = client;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public async Task<Charity> GetCharity(int registrationNumber)
        {
            var dto = await _client.GetCharity(registrationNumber);
            var result = _mapper.Map<ReferenceData.Api.Client.Dto.Charity, Charity>(dto);
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

        public async Task<PagedResponse<Organisation>> SearchOrganisations(string searchTerm, int pageNumber = 1, int pageSize = 25, OrganisationType? organisationType = null)
        {
            var result = await SearchOrganisations(searchTerm);

            if (result == null)
            {
                return new PagedResponse<Organisation>();
            }
            
            if (organisationType != null)
            {
                result = FilterOrganisationsByType(result, organisationType.Value);
            }
            
            return CreatePagedOrganisationResponse(pageNumber, pageSize, result);
        }

        private List<Organisation> SortOrganisations(List<Organisation> result, string searchTerm)
        {
            var outputList = new List<Organisation>();

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
        private void AddOrganisationsLooselyMatchingSearchByPosition(List<Organisation> rawOrganisations, string searchTerm, List<Organisation> outputList)
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
        private static void AgregateLocationAwareMatchesToOutList(ICollection<Organisation> outputList, IReadOnlyCollection<KeyValuePair<int, Organisation>> locationAwareMatches)
        {
            if (locationAwareMatches == null || !locationAwareMatches.Any())
                return;

            var maxLocation = locationAwareMatches.Max(m => m.Key);
            var i = 0;
            while (i <= maxLocation)
            {
                if (locationAwareMatches.Any(m => m.Key == i))
                {
                    var alphabeticOrganisationsAtIndex =
                        locationAwareMatches.Where(m => m.Key == i).ToList().OrderBy(m => m.Value.Name);

                    foreach (var item in alphabeticOrganisationsAtIndex)
                    {
                        if (outputList.Contains(item.Value))
                            continue;

                        outputList.Add(item.Value);
                    }
                }

                i++;
            }
        }

        private static List<KeyValuePair<int, Organisation>> FindLocationAwareMatches(List<Organisation> result, Regex rgx)
        {
            var locationAwareMatches = new List<KeyValuePair<int, Organisation>>();

            foreach (var item in result)
            {
                var matches = rgx.Matches(item.Name);
                if (matches.Count <= 0)
                    continue;

                locationAwareMatches.Add(new KeyValuePair<int, Organisation>(matches[0].Index, item));
            }
            return locationAwareMatches;
        }

        private static void AddResultsMatchingRegEx(List<Organisation> result, string priorityRegEx, List<Organisation> sortedList)
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

        private List<Organisation> FilterOrganisationsByType(IEnumerable<Organisation> result, OrganisationType organisationType)
        {
            if (organisationType == OrganisationType.Other || organisationType == OrganisationType.PublicBodies)
            {
                return result.Where(x => x.Type == OrganisationType.Other || x.Type == OrganisationType.PublicBodies).ToList();
            }
            return result.Where(x => x.Type == organisationType).ToList();
        }

        private async Task<List<Organisation>> SearchOrganisations(string searchTerm)
        {
            var cacheKey = $"SearchKey_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(searchTerm))}";

            var result = _cacheProvider.Get<List<Organisation>>(cacheKey);
            if (result == null)
            {
                //var orgs = await _client.SearchOrganisations(searchTerm);
                var processedSearchTerm = CleanSearchTerm(searchTerm);
                var orgs = await _client.SearchOrganisations(processedSearchTerm);

                if (orgs != null)
                {
                    var convertedOrgs = orgs.Select(ConvertToOrganisation).ToList();

                    result = SortOrganisations(convertedOrgs, searchTerm);

                    _cacheProvider.Set(cacheKey, result, new TimeSpan(0, 15, 0));
                }
            }
            return result;
        }

        private string CleanSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return searchTerm;

            foreach (var termToRemove in _termsToRemove)
            {
                if (!searchTerm.ToLower().EndsWith(termToRemove.ToLower()))
                    continue;

                var index = searchTerm.ToLower().LastIndexOf(termToRemove.ToLower(), StringComparison.Ordinal);

                searchTerm = searchTerm.Substring(0, index).TrimEnd();
            }

            return searchTerm;
        }

        private static PagedResponse<Organisation> CreatePagedOrganisationResponse(int pageNumber, int pageSize, List<Organisation> result)
        {
            return new PagedResponse<Organisation>
            {
                Data = result.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList(),
                TotalPages = (int)Math.Ceiling(((decimal) result.Count / pageSize)),
                PageNumber = pageNumber,
                TotalResults = result.Count
            };
        }

        private Organisation ConvertToOrganisation(ReferenceData.Api.Client.Dto.Organisation source)
        {
            return new Organisation
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
                Type = ConvertToOrganisationType(source.Type)
            };
        }

        private OrganisationType ConvertToOrganisationType(ReferenceData.Api.Client.Dto.OrganisationType sourceType)
        {
            switch (sourceType)
            {
                case ReferenceData.Api.Client.Dto.OrganisationType.Charity:
                    return OrganisationType.Charities;
                case ReferenceData.Api.Client.Dto.OrganisationType.Company:
                    return OrganisationType.CompaniesHouse;
                case ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation:
                    return OrganisationType.PublicBodies;
                case ReferenceData.Api.Client.Dto.OrganisationType.PublicSector:
                    return OrganisationType.PublicBodies;
                default:
                    return OrganisationType.Other;
            }
        }

        private OrganisationSubType ConvertToOrganisationSubType(ReferenceData.Api.Client.Dto.OrganisationSubType sourceSubType)
        {
            switch (sourceSubType)
            {
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Nhs:
                    return OrganisationSubType.Nhs;
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Ons:
                    return OrganisationSubType.Ons;
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Police:
                    return OrganisationSubType.Police;
                default:
                    return 0;
            }
        }
    }
}
