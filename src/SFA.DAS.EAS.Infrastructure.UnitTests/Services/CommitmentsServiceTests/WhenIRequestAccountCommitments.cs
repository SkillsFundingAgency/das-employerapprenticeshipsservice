using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.CommitmentsServiceTests
{
    public class WhenIRequestAccountCommitments
    {
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        private CommitmentService _service;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _service = new CommitmentService(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenIShouldGetCommitments()
        {
            //Arrange
            const int accountId = 4;
            var commitmentItem = new CommitmentListItem {Id = 2};

            _commitmentsApi.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(
                new List<CommitmentListItem>
                {
                    commitmentItem
                });

            //Act
            var result = await _service.GetEmployerCommitments(accountId);

            //Assert
            _commitmentsApi.Verify(x => x.GetEmployerCommitments(accountId), Times.Once);
            Assert.AreEqual(1,result.Count);
            Assert.AreEqual(commitmentItem.Id, result.First().Id);
        }

        [Test]
        public async Task ThenIShouldGetNoCommitmentsIfThereAreNone()
        {
            //Arrange
            _commitmentsApi.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(new List<CommitmentListItem>());

            //Act
            var result = await _service.GetEmployerCommitments(4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task ThenIShouldGetNoCommitmentsIfApiReturnsNull()
        {
            //Arrange
            _commitmentsApi.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(() => null);

            //Act
            var result = await _service.GetEmployerCommitments(4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
