using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPublicSectorOrgainsationTests
{
    public  class WhenIGetPublicSectorOrgainsations : QueryBaseTest<
        GetPublicSectorOrgainsationHandler,
        GetPublicSectorOrgainsationQuery,
        GetPublicSectorOrgainsationResponse>
    {
        private Mock<IReferenceDataService> _referenceDataServiceMock;
        private PagedResponse<PublicSectorOrganisation> _organisations;
        public override GetPublicSectorOrgainsationQuery Query { get; set; }
        public override GetPublicSectorOrgainsationHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPublicSectorOrgainsationQuery>> RequestValidator { get; set; }

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

            Query = new GetPublicSectorOrgainsationQuery
            {
                SearchTerm = "test"
            };
            RequestHandler = new GetPublicSectorOrgainsationHandler(
                RequestValidator.Object,
                _referenceDataServiceMock.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _referenceDataServiceMock.Verify(x => x.SearchPublicSectorOrganisation(Query.SearchTerm), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_organisations, response.Organisaions);
        }

        [Test]
        public async Task ThenShouldCallServiceWithPageNumberAndSizeIfProvided()
        {
            //Arrange
            Query.PageNumber = 2;
            Query.PageSize = 443;

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _referenceDataServiceMock.Verify(x => 
                x.SearchPublicSectorOrganisation(
                    Query.SearchTerm, 
                    Query.PageNumber.Value, 
                    Query.PageSize.Value), 
                    Times.Once);
        }

        [Test]
        public async Task ThenShouldCallServiceWithPageNumberIfProvided()
        {
            //Arrange
            Query.PageNumber = 2;

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _referenceDataServiceMock.Verify(x =>
                x.SearchPublicSectorOrganisation(
                    Query.SearchTerm,
                    Query.PageNumber.Value),
                    Times.Once);
        }
    }
}
