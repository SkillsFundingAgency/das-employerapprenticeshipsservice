﻿using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenICallRenameAccount
    {
        private Mock<IEmployerAccountRepository> _repository;
        private Mock<IValidator<RenameEmployerAccountCommand>> _validator;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
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

            _mediator = new Mock<IMediator>();

            _commandHandler = new RenameEmployerAccountCommandHandler(_repository.Object, _validator.Object, _hashingService.Object, _mediator.Object);
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
            _mediator.Verify(x => x.PublishAsync(It.Is<CreateAccountEventCommand>(e => e.HashedAccountId == HashedAccountId && e.Event == "AccountRenamed")));
        }

    }
}
