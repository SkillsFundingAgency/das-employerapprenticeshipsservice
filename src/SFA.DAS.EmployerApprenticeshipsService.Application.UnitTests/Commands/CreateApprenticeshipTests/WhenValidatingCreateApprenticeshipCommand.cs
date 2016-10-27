using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.Commitments.Api.Types;
using Moq;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeshipTests
{
    [TestFixture]
    public sealed class WhenValidatingCreateApprenticeshipCommand
    {
        private CreateApprenticeshipCommand _validCommand;
        private CreateApprenticeshipCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _handler = new CreateApprenticeshipCommandHandler(Mock.Of<ICommitmentsApi>());
            _validCommand = new CreateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship()
            };
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void AccountIdMustBeGreaterThanZeroElseExceptionThrown(long accountId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = accountId, Apprenticeship = new Apprenticeship { CommitmentId = 123 } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void CommitmentIdMustBeGreaterThanZeroElseExceptionThrown(long commitmentId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = commitmentId } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [Test]
        public void OnlyValidCommitmentIdAndAccountIdNeedsToBePopulated()
        {
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = 321 } };

            Assert.DoesNotThrowAsync(() => _handler.Handle(command));
        }
    }
}
