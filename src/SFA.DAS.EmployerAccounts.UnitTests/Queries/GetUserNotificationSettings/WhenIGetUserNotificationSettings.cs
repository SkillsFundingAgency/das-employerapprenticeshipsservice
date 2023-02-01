using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserNotificationSettings
{
    [TestFixture]
    public class WhenIGetUserNotificationSettings
    {
        private GetUserNotificationSettingsQueryHandler _handler;
        private Mock<IValidator<GetUserNotificationSettingsQuery>> _validator;
        private Mock<IAccountRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetUserNotificationSettingsQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetUserNotificationSettingsQuery>()))
                .Returns(() => new ValidationResult());

            _repository = new Mock<IAccountRepository>();
            _repository.Setup(x => x.GetUserAccountSettings(It.IsAny<string>()))
                .ReturnsAsync(new List<UserNotificationSetting>());

            _handler = new GetUserNotificationSettingsQueryHandler(_repository.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheQueryIsValidated()
        {
            //Arrange
            var query = new GetUserNotificationSettingsQuery();

            //Act
            await _handler.Handle(query, CancellationToken.None);

            //Assert
            _validator.Verify(x => x.Validate(query), Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledToRetrieveSettings()
        {
            //Arrange
            var query = new GetUserNotificationSettingsQuery
            {
                UserRef = "REF"
            };

            //Act
            await _handler.Handle(query, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.GetUserAccountSettings(
                It.Is<string>(s => s == "REF")));
        }
    }
}
