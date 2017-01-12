using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenICallRenameAccount
    {
        private Mock<IEmployerAccountRepository> _repository;
        private Mock<IValidator<RenameEmployerAccountCommand>> _validator;
        private Mock<IHashingService> _hashingService;
        private Mock<IEventsApi> _eventsApi;
        private const long AccountId = 12343322;
        private const string HashedAccountId = "123ADF23";
        private RenameEmployerAccountCommandHandler _commandHandler;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.Is<string>(s => s == HashedAccountId)))
                .Returns(AccountId);

            _repository = new Mock<IEmployerAccountRepository>();
            _validator = new Mock<IValidator<RenameEmployerAccountCommand>>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<RenameEmployerAccountCommand>()))
                .ReturnsAsync(new ValidationResult());

            _eventsApi = new Mock<IEventsApi>();

            _commandHandler = new RenameEmployerAccountCommandHandler(_repository.Object, _validator.Object, _hashingService.Object, _eventsApi.Object);
        }

        [Test]
        public async Task ThenTheAccountIsRenamedToTheNewName()
        {
            //Arrange
            var newAccountName = "Renamed account";
            var command = new RenameEmployerAccountCommand
            {
                HashedAccountId = HashedAccountId,
                NewName = newAccountName
            };

            //Act
            await _commandHandler.Handle(command);

            //Assert
            _repository.Verify(x=> x.RenameAccount(It.Is<long>(l=> l == AccountId), It.Is<string>(s => s== newAccountName)));
            _eventsApi.Verify(x => x.CreateAccountEvent(It.Is<AccountEvent>(e => e.EmployerAccountId == HashedAccountId && e.Event == "AccountRenamed")));
        }

    }
}
