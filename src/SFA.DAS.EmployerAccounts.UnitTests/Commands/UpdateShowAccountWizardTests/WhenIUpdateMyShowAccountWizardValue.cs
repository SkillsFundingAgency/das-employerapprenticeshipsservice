using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UpdateShowAccountWizardTests
{
    [TestFixture]
    public class WhenIUpdateMyShowAccountWizardValue
    {
        private Mock<IMembershipRepository> _memberRepository;
        private UpdateShowAccountWizardCommandHandler _handler;
        private Mock<ILogger<UpdateShowAccountWizardCommandHandler>> _logger;
        private Mock<IValidator<UpdateShowAccountWizardCommand>> _validator;
        private UpdateShowAccountWizardCommand _command;

        [SetUp]
        public void Arrange()
        {
            _memberRepository = new Mock<IMembershipRepository>();
            _validator = new Mock<IValidator<UpdateShowAccountWizardCommand>>();
            _logger = new Mock<ILogger<UpdateShowAccountWizardCommandHandler>>();

            _handler = new UpdateShowAccountWizardCommandHandler(_memberRepository.Object, _validator.Object, _logger.Object);
            _command = new UpdateShowAccountWizardCommand
            {
                HashedAccountId = "123ABC",
                ExternalUserId = "HJKJH",
                ShowWizard = true
            };
        }

        [Test]
        public async Task ShouldCallRepositoryIfCommandIsValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UpdateShowAccountWizardCommand>()))
                      .Returns(new ValidationResult());
            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _memberRepository.Verify(x => x.SetShowAccountWizard(_command.HashedAccountId, 
                _command.ExternalUserId, _command.ShowWizard), Times.Once);
        }

        [Test]
        public void ShouldNotCallRepositoryIfCommandIsInvalid()
        {
            //Arrange
            var validationResult = new ValidationResult();
            validationResult.AddError("test");

            _validator.Setup(x => x.Validate(It.IsAny<UpdateShowAccountWizardCommand>()))
                      .Returns(validationResult);

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => { await _handler.Handle(_command, CancellationToken.None); });

            //Assert
            _memberRepository.Verify(x => x.SetShowAccountWizard(It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
