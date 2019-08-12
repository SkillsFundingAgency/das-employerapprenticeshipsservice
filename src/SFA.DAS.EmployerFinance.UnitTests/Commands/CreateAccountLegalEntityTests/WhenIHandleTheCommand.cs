﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.CreateAccountLegalEntityTests
{
    public class WhenIHandleTheCommand
    {
        private CreateAccountLegalEntityCommandHandler _handler;
        private Mock<IAccountLegalEntityRepository> _accountLegalEntityRepository;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _accountLegalEntityRepository = new Mock<IAccountLegalEntityRepository>();

            _logger = new Mock<ILog>();

            _handler = new CreateAccountLegalEntityCommandHandler(_accountLegalEntityRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheAccountLegalEntityIsCreated()
        {
            var id = 337782;
            var deleted = false;
            var pendingAgreementId = 56832;
            var signedAgreementId = 338921;
            var signedAgreementVersion = 3;
            var accountId = 10862;

            await _handler.Handle(new CreateAccountLegalEntityCommand(id, deleted, pendingAgreementId, signedAgreementId, signedAgreementVersion, accountId));

            _accountLegalEntityRepository.Verify(x => x.CreateAccountLegalEntity(id, deleted, pendingAgreementId, signedAgreementId, signedAgreementVersion, accountId));
        }
    }
}