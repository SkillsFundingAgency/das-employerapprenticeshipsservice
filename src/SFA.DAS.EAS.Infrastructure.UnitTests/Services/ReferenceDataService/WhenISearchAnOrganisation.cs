using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.ReferenceData.Api.Client.Dto;
using FluentAssertions;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ReferenceDataService
{
    public class WhenISearchAnOrganisation
    {
        private Mock<IReferenceDataApiClient> _apiClient;
        private Infrastructure.Services.ReferenceDataService _referenceDataService;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _apiClient = new Mock<IReferenceDataApiClient>();
            _mapper = new Mock<IMapper>();

            _referenceDataService = new Infrastructure.Services.ReferenceDataService(_apiClient.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenTheClientIsCalledWithTheSearchTerm()
        {
            //Arrange
            var expectedSearchTerm = "Some Org";

            //Act
            await _referenceDataService.SearchOrganisations(expectedSearchTerm);

            //Assert
            _apiClient.Verify(x => x.SearchOrganisations(expectedSearchTerm, 1, 20, 500));
        }

        [Test]
        public async Task ThenTheDataIsReturnedFromTheApi()
        {
            //Arrange
            _mapper.Setup(x => x.Map<Domain.Models.ReferenceData.Organisation>(It.IsAny<Organisation>()))
                .Returns(new Domain.Models.ReferenceData.Organisation {
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
            var expectedOrganisation = new Organisation
            {
                Name = "Company Name",
                Type = OrganisationType.Company,
                Address = new Address
                {
                    Line1 = "test 1",
                    Line2 = "test 2",
                    Line3 = "test 3",
                    Line4 = "test 4",
                    Line5 = "test 5",
                    Postcode = "Test code"
                },
                Code = "ABC123",
                RegistrationDate = new DateTime(2016,10,15),
                Sector = "sector",
                SubType = OrganisationSubType.Police
            };
            _apiClient.Setup(x => x.SearchOrganisations(expectedSearchTerm, 1, 20, 500))
                .ReturnsAsync(new PagedApiResponse<Organisation>
                {
                    Data = new List<Organisation>
                    {
                        expectedOrganisation
                    }
                });

            //Act
            var actual = await _referenceDataService.SearchOrganisations(expectedSearchTerm);

            //Assert
            actual.FirstOrDefault().ShouldBeEquivalentTo(expectedOrganisation);
            Assert.IsAssignableFrom<List<Domain.Models.ReferenceData.Organisation>>(actual);
        }
     }
}
