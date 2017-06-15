﻿using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLockSummary;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetApprenticeshipDataLockSummary
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLockSummary
    {
        private GetDataLockSummaryQueryHandler _handler;
        private Mock<IDataLockApi> _dataLockApi;

        [SetUp]
        public void Arrange()
        {
            _dataLockApi = new Mock<IDataLockApi>();
            _dataLockApi.Setup(x => x.GetDataLockSummary(It.IsAny<long>()))
                .ReturnsAsync(new DataLockSummary());

            _handler = new GetDataLockSummaryQueryHandler(_dataLockApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToRetrieveDataLocks()
        {
            //Arrange
            var request = new GetDataLockSummaryQueryRequest
            {
                ApprenticeshipId = 1
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _dataLockApi.Verify(x => x.GetDataLockSummary(It.IsAny<long>()), Times.Once);
        }
    }
}
