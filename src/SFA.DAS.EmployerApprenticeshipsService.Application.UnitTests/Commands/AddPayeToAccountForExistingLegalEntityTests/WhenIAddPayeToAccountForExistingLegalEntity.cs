using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.AddPayeToAccountForExistingLegalEntityTests
{
    [TestFixture]
    public class WhenIAddPayeToAccountForExistingLegalEntity
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private AddPayeToAccountForExistingLegalEntityCommandHandler _handler;
        private AddPayeToAccountForExistingLegalEntityCommand _command;
        private Mock<IMessagePublisher> _messagePublisher;

        [SetUp]
        public void Setup()
        {

            _command = new AddPayeToAccountForExistingLegalEntityCommand
            {
                AccountId = 1,
                LegalEntityId = 2,
                EmpRef = "123/ABC",
                ExternalUserId = Guid.NewGuid().ToString(),
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
            };

            _messagePublisher = new Mock<IMessagePublisher>();

            _accountRepository = new Mock<IAccountRepository>(); 
            _membershipRepository = new Mock<IMembershipRepository>();
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(x => x.GetLegalEntitiesLinkedToAccount(_command.AccountId))
                .ReturnsAsync(new List<LegalEntity> { new LegalEntity { Id = 2 } });

            _handler = new AddPayeToAccountForExistingLegalEntityCommandHandler(_accountRepository.Object, _membershipRepository.Object, _employerAgreementRepository.Object, _messagePublisher.Object);

            

            _membershipRepository.Setup(x => x.GetCaller(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                RoleId = (short)Role.Owner
            });
        }

        [Test]
        public void ThenIThrowAnExceptionWhenValidationFails()
        {
            _command = new AddPayeToAccountForExistingLegalEntityCommand();

            var requestException = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(requestException.ErrorMessages.Count, Is.EqualTo(6));

            Assert.True(requestException.ErrorMessages.ContainsKey("AccountId"));
            Assert.True(requestException.ErrorMessages.ContainsKey("LegalEntityId"));
            Assert.True(requestException.ErrorMessages.ContainsKey("EmpRef"));
            Assert.True(requestException.ErrorMessages.ContainsKey("ExternalUserId"));
            Assert.True(requestException.ErrorMessages.ContainsKey("AccessToken"));
            Assert.True(requestException.ErrorMessages.ContainsKey("RefreshToken"));
        }

        [Test]
        public void ThenIThrowAnExceptionWhenNotAMember()
        {
            _membershipRepository.Setup(x => x.GetCaller(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(null);

            var requestException = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(requestException.ErrorMessages.Count, Is.EqualTo(1));
            Assert.True(requestException.ErrorMessages.ContainsKey("Membership"));
        }

        [Test]
        public void ThenIThrowAnExceptionWhenMemberNotAnOwner()
        {
            _membershipRepository.Setup(x => x.GetCaller(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                RoleId = (short)Role.Viewer
            });

            var requestException = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(requestException.ErrorMessages.Count, Is.EqualTo(1));
            Assert.True(requestException.ErrorMessages.ContainsKey("Membership"));
        }

        [Test]
        public void ThenIThrowAnExceptionWhenLegalEntityNotLinkedToAccount()
        {
            _employerAgreementRepository.Setup(x => x.GetLegalEntitiesLinkedToAccount(_command.AccountId))
                .ReturnsAsync(new List<LegalEntity>());

            var requestException = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(requestException.ErrorMessages.Count, Is.EqualTo(1));
            Assert.True(requestException.ErrorMessages.ContainsKey("LegalEntity"));
        }

        [Test]
        public async Task ThenISuccessfullyAddPaye()
        {
            //Act
            await _handler.Handle(_command);
            
            //Assert
            _accountRepository.Verify(x => x.AddPayeToAccountForExistingLegalEntity(_command.AccountId, _command.LegalEntityId, _command.EmpRef, _command.AccessToken, _command.RefreshToken), Times.Once);
        }

        [Test]
        public async Task ThenAMessageIsQueuedToRefreshTheLevyData()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c => c.AccountId.Equals(_command.AccountId))));
        }
    }
}