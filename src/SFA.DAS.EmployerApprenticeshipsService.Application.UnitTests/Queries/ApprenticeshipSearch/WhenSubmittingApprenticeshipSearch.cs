using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EAS.Domain.Interfaces;
using FluentAssertions;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.ApprenticeshipSearch
{
    [TestFixture()]
    public class WhenSubmittingApprenticeshipSearch
    {
        private ApprenticeshipSearchQueryHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(
                x => x.GetEmployerApprenticeships(It.IsAny<long>(), It.IsAny<ApprenticeshipSearchQuery>()))
                .ReturnsAsync(new ApprenticeshipSearchResponse
                {
                    Apprenticeships = new List<Apprenticeship>(),
                    PageNumber = 2,
                    PageSize = 10,
                    TotalApprenticeships = 100
                });

            _handler = new ApprenticeshipSearchQueryHandler(_commitmentsApi.Object, Mock.Of<IHashingService>());
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalled()
        {
            //Arrange
            var request = new ApprenticeshipSearchQueryRequest
            {
                HashedLegalEntityId = "EmployerId",
                Query = new ApprenticeshipSearchQuery()
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _commitmentsApi.Verify(
                x => x.GetEmployerApprenticeships(
                    It.IsAny<long>(),
                    It.IsAny<ApprenticeshipSearchQuery>()),
                    Times.Once);
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalledWithCorrectPageNumber()
        {
            //Arrange
            var request = new ApprenticeshipSearchQueryRequest
            {
                HashedLegalEntityId = "EmployerId",
                Query = new ApprenticeshipSearchQuery { PageNumber = 5 }
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _commitmentsApi.Verify(
                x => x.GetEmployerApprenticeships(
                    It.IsAny<long>(),
                    It.Is<ApprenticeshipSearchQuery>(a => a.PageNumber == 5)),
                    Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnPaginationValuesFromApi()
        {
            //Arrange
            var request = new ApprenticeshipSearchQueryRequest
            {
                HashedLegalEntityId = "EmployerId",
                Query = new ApprenticeshipSearchQuery()
            };

            //Act
            var response = await _handler.Handle(request);

            //Assert
            response.PageNumber.Should().Be(2);
            response.PageSize.Should().Be(10);
            response.TotalApprenticeships.Should().Be(100);
        }
    }
}
