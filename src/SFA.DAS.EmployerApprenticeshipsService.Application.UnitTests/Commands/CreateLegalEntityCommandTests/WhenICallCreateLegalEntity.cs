using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenICallCreateLegalEntity
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private CreateLegalEntityCommandHandler _commandHandler;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _mediator = new Mock<IMediator>();

            _commandHandler = new CreateLegalEntityCommandHandler(_accountRepository.Object, _membershipRepository.Object, _mediator.Object);
        }

        [Test]
        public async Task ThenTheAccountIsRenamedToTheNewName()
        {
            //Arrange
            var command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                LegalEntity = new LegalEntity(),
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = "12345"
            };

            var owner = new MembershipView { AccountId = 1234, UserId = 9876 };
            _membershipRepository.Setup(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId)).ReturnsAsync(owner);
            var agreementView = new EmployerAgreementView();
            _accountRepository.Setup(x => x.CreateLegalEntity(owner.AccountId, command.LegalEntity, command.SignAgreement, command.SignedDate, owner.UserId)).ReturnsAsync(agreementView);

            //Act
            var result = await _commandHandler.Handle(command);

            //Assert
            Assert.AreSame(agreementView, result.AgreementView);
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAccountEventCommand>(e => e.HashedAccountId == command.HashedAccountId && e.Event == "LegalEntityCreated")), Times.Once);
        }

    }
}
