using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpsertRegisteredUserTests
{
    public class WhenUpsertingAUser
    {
        private Mock<IValidator<UpsertRegisteredUserCommand>> _validator;
        private Mock<IUserRepository> _userRepository;
        private UpsertRegisteredUserCommandHandler _handler;
        private UpsertRegisteredUserCommand _command;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpsertRegisteredUserCommand>>();
            _validator.Setup(v => v.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
                .Returns(new ValidationResult());

            _userRepository = new Mock<IUserRepository>();

            _handler = new UpsertRegisteredUserCommandHandler(_validator.Object, _userRepository.Object);

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
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenItShouldUpsertTheUserInTheRepository()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _userRepository.Verify(r => r.Upsert(It.Is<User>(u => u.Email == _command.EmailAddress
                                                               && u.UserRef == _command.UserRef
                                                               && u.FirstName == _command.FirstName
                                                               && u.LastName == _command.LastName)), Times.Once);
        }
    }
}
