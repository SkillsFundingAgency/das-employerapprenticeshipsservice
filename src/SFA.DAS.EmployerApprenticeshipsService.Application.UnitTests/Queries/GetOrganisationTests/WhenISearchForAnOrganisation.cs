using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetOrganisationTests
{
    public class WhenISearchForAnOrganisation :QueryBaseTest<GetOrganisationsQueryHandler, GetOrganisationsRequest, GetOrganisationsResponse>
    {
        private Mock<IReferenceDataService> _referenceDataService;
        public override GetOrganisationsRequest Query { get; set; }
        public override GetOrganisationsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetOrganisationsRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _referenceDataService = new Mock<IReferenceDataService>();

            Query = new GetOrganisationsRequest {SearchTerm ="Company Search"};

            RequestHandler = new GetOrganisationsQueryHandler(RequestValidator.Object, _referenceDataService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            var expectedSearchTerm = "My Company";

            //Act
            await RequestHandler.Handle(new GetOrganisationsRequest {SearchTerm = expectedSearchTerm });

            //Assert
            _referenceDataService.Verify(x=>x.SearchOrganisations(expectedSearchTerm), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var expectedSearchTerm = "My Company";
            _referenceDataService.Setup(x => x.SearchOrganisations(expectedSearchTerm))
                .ReturnsAsync(new List<Organisation> {new Organisation()});

            //Act
            var actual = await RequestHandler.Handle(new GetOrganisationsRequest { SearchTerm = expectedSearchTerm });

            //Assert
            Assert.IsNotNull(actual.Organisations);
            Assert.AreEqual(1,actual.Organisations.Count);
        }
    }
}
