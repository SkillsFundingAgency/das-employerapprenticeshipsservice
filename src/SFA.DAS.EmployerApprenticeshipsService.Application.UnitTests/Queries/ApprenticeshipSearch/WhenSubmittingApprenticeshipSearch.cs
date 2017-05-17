using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EAS.Domain.Interfaces;

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
                    Apprenticeships = new List<Apprenticeship>()
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

    }
}
