using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.ReferenceData.Api.Client;
using FluentAssertions;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ReferenceDataService
{
    public class WhenISearchAnOrganisation
    {
        private Mock<IReferenceDataApiClient> _apiClient;
        private Infrastructure.Services.ReferenceDataService _referenceDataService;
        private Mock<IMapper> _mapper;
        private Mock<ICacheProvider> _cacheProvider;
        private ReferenceData.Api.Client.Dto.Organisation _expectedOrganisation;

        [SetUp]
        public void Arrange()
        {

            _apiClient = new Mock<IReferenceDataApiClient>();
            _mapper = new Mock<IMapper>();


            _mapper.Setup(x => x.Map<Organisation>(It.IsAny<ReferenceData.Api.Client.Dto.Organisation>()))
               .Returns(new Organisation
               {
                   Name = "Company Name",
                   Type = Domain.Models.Organisation.OrganisationType.CompaniesHouse,
                   Address = new Domain.Models.Organisation.Address
                   {
                       Line1 = "test 1",
                       Line2 = "test 2",
                       Line3 = "test 3",
                       Line4 = "test 4",
                       Line5 = "test 5",
                       Postcode = "Test code"
                   },
                   Code = "ABC123",
                   RegistrationDate = new DateTime(2016, 10, 15),
                   Sector = "sector",
                   SubType = Domain.Models.Organisation.OrganisationSubType.Police
               });
            var expectedSearchTerm = "Some Org";
            _expectedOrganisation = ArrangeOrganisation();
            _apiClient.Setup(x => x.SearchOrganisations(expectedSearchTerm, 500))
                .ReturnsAsync(
                    new List<ReferenceData.Api.Client.Dto.Organisation>
                    {
                        _expectedOrganisation
                    }
                );

            _cacheProvider = new Mock<ICacheProvider>();

            _referenceDataService = new Infrastructure.Services.ReferenceDataService(_apiClient.Object, _mapper.Object, _cacheProvider.Object);
        }

        [Test]
        public async Task ThenTheClientIsCalledWithTheSearchTerm()
        {
            //Arrange
            var expectedSearchTerm = "Some Org";

            //Act
            await _referenceDataService.SearchOrganisations(expectedSearchTerm);

            //Assert
            _apiClient.Verify(x => x.SearchOrganisations(expectedSearchTerm, 500), Times.Once);
        }

        [Test]
        public async Task ThenTheDataIsReturnedFromTheApi()
        {
            //Arrange
            var expectedSearchTerm = "Some Org";

            //Act
            var actual = await _referenceDataService.SearchOrganisations(expectedSearchTerm);

            //Assert
            actual.Data.FirstOrDefault().ShouldBeEquivalentTo(_expectedOrganisation);
            Assert.IsAssignableFrom<PagedResponse<Organisation>>(actual);
        }

        [Test]
        public async Task ThenSubsequentCallsAreRetrievedFromTheApi()
        {
            //Arrange
            var expectedSearchTerm = "Some Org";
            var searchKey = $"SearchKey_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(expectedSearchTerm))}";
            _cacheProvider.SetupSequence(c => c.Get<List<Organisation>>(searchKey))
                .Returns(null)
                .Returns(new List<Organisation> { new Organisation() });

            //Act
            await _referenceDataService.SearchOrganisations(expectedSearchTerm);
            await _referenceDataService.SearchOrganisations(expectedSearchTerm);

            //Assert
            _apiClient.Verify(x => x.SearchOrganisations(expectedSearchTerm, 500), Times.Once);
            _cacheProvider.Verify(x => x.Get<List<Organisation>>(searchKey), Times.Exactly(2));
            _cacheProvider.Verify(x => x.Set(searchKey, It.Is<List<Organisation>>(c => c != null), It.Is<TimeSpan>(c => c.Minutes.Equals(15))), Times.Once);
        }

        [Test]
        public async Task ThenTheResultsArePaged()
        {
            //Arrange
            var expectedSearchTerm = "Some Org";
            var organisations = new List<ReferenceData.Api.Client.Dto.Organisation>();
            for (var i = 0; i <= 50; i++)
            {
                organisations.Add(ArrangeOrganisation());
            }
            _apiClient.Setup(x => x.SearchOrganisations(expectedSearchTerm, 500))
               .ReturnsAsync(
                   organisations
               );

            //Act
            var actual = await _referenceDataService.SearchOrganisations(expectedSearchTerm,2);

            //Assert
            Assert.AreEqual(2, actual.PageNumber);
            Assert.AreEqual(3, actual.TotalPages);
            Assert.AreEqual(25, actual.Data.Count);
        }

        private static ReferenceData.Api.Client.Dto.Organisation ArrangeOrganisation()
        {
            return new ReferenceData.Api.Client.Dto.Organisation
            {
                Name = "Company Name",
                Type = ReferenceData.Api.Client.Dto.OrganisationType.Company,
                Address = new ReferenceData.Api.Client.Dto.Address
                {
                    Line1 = "test 1",
                    Line2 = "test 2",
                    Line3 = "test 3",
                    Line4 = "test 4",
                    Line5 = "test 5",
                    Postcode = "Test code"
                },
                Code = "ABC123",
                RegistrationDate = new DateTime(2016, 10, 15),
                Sector = "sector",
                SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.Police
            };
        }
    }
}
