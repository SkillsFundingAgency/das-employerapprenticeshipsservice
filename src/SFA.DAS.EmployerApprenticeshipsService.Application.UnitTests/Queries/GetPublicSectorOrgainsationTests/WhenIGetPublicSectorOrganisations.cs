using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPublicSectorOrgainsationTests
{
    public  class WhenIGetPublicSectorOrganisations : QueryBaseTest<
        GetPublicSectorOrganisationHandler,
        GetPublicSectorOrganisationQuery,
        GetPublicSectorOrganisationResponse>
    {
        private Mock<IReferenceDataService> _referenceDataServiceMock;
        private PagedResponse<PublicSectorOrganisation> _organisations;
        public override GetPublicSectorOrganisationQuery Query { get; set; }
        public override GetPublicSectorOrganisationHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPublicSectorOrganisationQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _organisations = new PagedResponse<PublicSectorOrganisation>();

            _referenceDataServiceMock = new Mock<IReferenceDataService>();

            _referenceDataServiceMock.Setup(x => x.SearchPublicSectorOrganisation(It.IsAny<string>()))
                                     .ReturnsAsync(_organisations);
            _referenceDataServiceMock.Setup(x => x.SearchPublicSectorOrganisation(
                                                    It.IsAny<string>(), It.IsAny<int>()))
                                     .ReturnsAsync(_organisations);
            _referenceDataServiceMock.Setup(x => x.SearchPublicSectorOrganisation(
                                                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                                     .ReturnsAsync(_organisations);

            Query = new GetPublicSectorOrganisationQuery
            {
                SearchTerm = "test",
                PageNumber = 2,
                PageSize = 223
            };
            RequestHandler = new GetPublicSectorOrganisationHandler(
                RequestValidator.Object,
                _referenceDataServiceMock.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _referenceDataServiceMock.Verify(x =>
                x.SearchPublicSectorOrganisation(
                    Query.SearchTerm,
                    Query.PageNumber,
                    Query.PageSize),
                    Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_organisations, response.Organisaions);
        }
    }
}
