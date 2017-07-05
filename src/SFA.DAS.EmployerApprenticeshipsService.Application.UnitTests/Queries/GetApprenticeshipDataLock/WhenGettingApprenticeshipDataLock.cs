using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLock;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetApprenticeshipDataLock
{
    [TestFixture]
    public sealed class WhenGettingApprenticeshipDataLock
    {
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        [Test]
        public async Task ShouldFilterForFailedDataLocks()
        {
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetDataLocks(666, 123L)).ReturnsAsync(new List<DataLockStatus>
            {
                new DataLockStatus { DataLockEventId = 1, Status = Commitments.Api.Types.DataLock.Types.Status.Pass },
                new DataLockStatus { DataLockEventId = 2, Status = Commitments.Api.Types.DataLock.Types.Status.Pass },
                new DataLockStatus { DataLockEventId = 3, Status = Commitments.Api.Types.DataLock.Types.Status.Fail },
                new DataLockStatus { DataLockEventId = 4, Status = Commitments.Api.Types.DataLock.Types.Status.Pass },
            });

            var handler = new GetApprenticeshipDataLockQueryHandler(_commitmentsApi.Object, Mock.Of<ILog>());

            var response = await handler.Handle(new GetApprenticeshipDataLockRequest { AccountId = 666, ApprenticeshipId = 123L });

            response.DataLockStatus.Count().Should().Be(1);
        }
    }
}
