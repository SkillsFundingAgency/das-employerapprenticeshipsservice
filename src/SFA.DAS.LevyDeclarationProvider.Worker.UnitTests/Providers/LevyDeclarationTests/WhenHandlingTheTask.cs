using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
{
    public class WhenHandlingTheTask
    {
        private LevyDeclaration _levyDeclaration;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;

        [SetUp]
        public void Arrange()
        {
            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();

            _levyDeclaration = new LevyDeclaration(_pollingMessageReceiver.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReceivedFromTheQueue()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert
            _pollingMessageReceiver.Verify(x=>x.ReceiveAsAsync<QueueMessage>(),Times.Once);
        }

        [Test]
        public async Task ThenTheDeclarationDataIsReceivedForTheQueueMessage()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert


        }

        [Test]
        public async Task ThenTheRebuildDeclarationCommandIsCalled()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert

        }

        [Test]
        public async Task ThenIfATooManyRequestsExceptionIsThrownItIsHandled()
        {
            //Act
            await _levyDeclaration.Handle();

        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Act
            await _levyDeclaration.Handle();

        }
    }
}
