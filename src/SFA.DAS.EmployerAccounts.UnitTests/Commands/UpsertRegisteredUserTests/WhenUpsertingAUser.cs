using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UpsertRegisteredUserTests
{
    public class WhenUpsertingAUser
    {
        private Mock<IValidator<UpsertRegisteredUserCommand>> _validator;
        private Mock<IUserAccountRepository> _userRepository;
        private Mock<IEventPublisher> _eventPublisher;
        private UpsertRegisteredUserCommandHandler _handler;
        private UpsertRegisteredUserCommand _command;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpsertRegisteredUserCommand>>();
            _validator.Setup(v => v.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
                .Returns(new ValidationResult());

            _userRepository = new Mock<IUserAccountRepository>();
            _eventPublisher = new Mock<IEventPublisher>();

            _handler = new UpsertRegisteredUserCommandHandler(_validator.Object, _userRepository.Object, _eventPublisher.Object);

            _command = new UpsertRegisteredUserCommand
            {
                UserRef = Guid.NewGuid().ToString(),
                FirstName = "User",
                LastName = "One",
                EmailAddress = "user.one@unit.tests"
            };
        }

        [Test]
        public void ThenItShouldThrowAnInvalidRequestExceptionIfTheCommandIsNotValid()
        {
            // Arrange
            _validator.Setup(v => v.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
                .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "Error" } } });

            // Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command, CancellationToken.None));
        }

        [Test]
        public async Task ThenItShouldUpsertTheUserInTheRepository()
        {
            // Act
            await _handler.Handle(_command, CancellationToken.None);

            // Assert
            _userRepository.Verify(r => r.Upsert(It.Is<User>(u => u.Email == _command.EmailAddress
                                                               && u.UserRef == _command.UserRef
                                                               && u.FirstName == _command.FirstName
                                                               && u.LastName == _command.LastName)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldPublishAnEventToReportTheUpsert()
        {
            await _handler.Handle(_command, CancellationToken.None);

            _eventPublisher.Verify(x => x.Publish(It.Is<UpsertedUserEvent>(y => y.UserRef == _command.UserRef)));
        }
    }
}
