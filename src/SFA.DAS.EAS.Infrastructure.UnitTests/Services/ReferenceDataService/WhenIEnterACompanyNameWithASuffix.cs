using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ReferenceDataService
{
    /// <summary>
    /// GIVEN I am searching for my organisation
    /// WHEN I enter a name including a suffix of "Limited" or "PLC" (Limited", "LIMITED", "Ltd", "LTD", "PLC" or "Plc")
    /// THEN the search ignores them AND returns names based on the name they have entered without reference to the suffix.
    /// </summary>
    public class WhenIEnterACompanyNameWithASuffix
    {
        private Mock<IReferenceDataApiClient> _apiClient;
        private Infrastructure.Services.ReferenceDataService _referenceDataService;
        private Mock<IMapper> _mapper;
        private Mock<IInProcessCache> _inProcessCache;

        [SetUp]
        public void Arrange()
        {
            _apiClient = new Mock<IReferenceDataApiClient>();
             _mapper = new Mock<IMapper>();
             _inProcessCache = new Mock<IInProcessCache>();

             _referenceDataService = new Infrastructure.Services.ReferenceDataService(_apiClient.Object, _mapper.Object, _inProcessCache.Object);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixLimited()
         {
             const string searchTerm = "Accomplish IT Limited";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixUcaseLimited()
         {
             const string searchTerm = "Accomplish IT LIMITED";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixLcaseLimited()
         {
             const string searchTerm = "Accomplish IT limited";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixLtd()
         {
             const string searchTerm = "Accomplish IT Ltd";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixUcaseLtd()
         {
             const string searchTerm = "Accomplish IT LTD";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixLcaseLtd()
         {
             const string searchTerm = "Accomplish IT ltd";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT LIMITED";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixPlc()
         {
             const string searchTerm = "Accomplish IT Plc";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT PLC";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixUcasePlc()
         {
             const string searchTerm = "Accomplish IT PLC";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT PLC";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

         [Test]
         public async Task ThenTheServiceIsCalledWithoutTheSuffixLcasePlc()
         {
             const string searchTerm = "Accomplish IT plc";
             const string wsSearchTerm = "Accomplish IT";
             const string wsResultName = "ACCOMPLISH IT PLC";

             //Arrange
             SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

             await _referenceDataService.SearchOrganisations(searchTerm);

             // ASSERT
             _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
         }

        [Test]
        public async Task ThenTheSortOrderIsUnAffectedByTheSuffix()
        {
            const string searchTerm = "Accomplish IT plc";
            const string wsSearchTerm = "Accomplish IT";
            const string wsResultName = "ACCOMPLISH IT PLC";

            //Arrange
            SetupWsSearchOrganistations(wsSearchTerm, wsResultName);

            await _referenceDataService.SearchOrganisations(searchTerm);

            // ASSERT
            _apiClient.Verify(x => x.SearchOrganisations(wsSearchTerm, 500), Times.Once);
        }

        private void SetupWsSearchOrganistations(string wsSearchTerm, string wsResultName)
         {
             _apiClient.Setup(x => x.SearchOrganisations(wsSearchTerm, 500))
                 .ReturnsAsync(
                     new List<ReferenceData.Api.Client.Dto.Organisation>
                     {
                         new ReferenceData.Api.Client.Dto.Organisation
                         {
                             Name = wsResultName,
                             Address = ConstructStandardAddressDto(),
                             Code = "ABC123",
                             RegistrationDate = new DateTime(2016, 10, 15),
                             Sector = "sector",
                             SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.Police
                         }
                     }
                 );
         }

         private static ReferenceData.Api.Client.Dto.Address ConstructStandardAddressDto()
         {
             return new ReferenceData.Api.Client.Dto.Address
             {
                 Line1 = "test 1",
                 Line2 = "test 2",
                 Line3 = "test 3",
                 Line4 = "test 4",
                 Line5 = "test 5",
                 Postcode = "Test code"
             };
         }
     }
}
