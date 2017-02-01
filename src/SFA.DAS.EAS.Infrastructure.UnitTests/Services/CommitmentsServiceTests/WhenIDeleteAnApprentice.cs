using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.CommitmentsServiceTests
{
    [TestFixture]
    public class WhenIDeleteAnApprentice
    {
        private CommitmentsService _service;
        private Mock<ICommitmentsApi> _commitmentsApi;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<ICommitmentsApi>();
            _commitmentsApi.Setup(x => x.DeleteEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<object>(null));

            _service = new CommitmentsService(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalled()
        {
            //Arrange
            long accountId = 1;
            long apprenticeId = 2;

            //Act
            await _service.DeleteEmployerApprenticeship(accountId, apprenticeId);

            //Assert            
            _commitmentsApi.Verify(x =>
                x.DeleteEmployerApprenticeship(It.Is<long>(l=> l==accountId), It.Is<long>(l=> l==apprenticeId))
                , Times.Once);
        }

    }
}
