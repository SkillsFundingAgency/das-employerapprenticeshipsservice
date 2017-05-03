using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Commands.CreateApprenticeship;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateApprenticeshipTests
{
    [TestFixture]
    public sealed class WhenCreatingAnApprenticeship
    {
        private CreateApprenticeshipCommand _validCommand;
        private CreateApprenticeshipCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentApi;

        [SetUp]
        public void Setup()
        {
            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _handler = new CreateApprenticeshipCommandHandler(_commitmentApi.Object);
            _validCommand = new CreateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship { CommitmentId = 5634 },
                UserId = "ABC123",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };
        }

        [Test]
        public async Task ThenTheApprenticeshipIsCreated()
        {
            await _handler.Handle(_validCommand);

            _commitmentApi.Verify(
                x =>
                    x.CreateEmployerApprenticeship(_validCommand.AccountId, _validCommand.Apprenticeship.CommitmentId,
                        It.Is<ApprenticeshipRequest>(
                            y =>
                                y.Apprenticeship == _validCommand.Apprenticeship && y.UserId == _validCommand.UserId && y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName &&
                                y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress)));
        }
    }
}
