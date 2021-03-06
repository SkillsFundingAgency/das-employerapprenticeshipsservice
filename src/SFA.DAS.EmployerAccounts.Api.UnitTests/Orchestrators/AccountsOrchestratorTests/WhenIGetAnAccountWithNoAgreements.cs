﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using AccountDetail = SFA.DAS.EmployerAccounts.Models.Account.AccountDetail;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAnAccountWithNoAgreements
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _log;

        [SetUp]
        public void Arrange()
        {      
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILog>();
            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, Mock.Of<IMapper>(), Mock.Of<IHashingService>());
        }

        [Test]
        public async Task ThenResponseShouldHaveAccountAgreementTypeSetToUnknown()
        {
            //Arrange
            var response = new GetEmployerAccountDetailByHashedIdResponse
            {
                Account = new AccountDetail
                {
                    AccountAgreementTypes = new List<AgreementType>()
                }
            };

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(response)
                .Verifiable("Get account was not called");

            //Act
            var result = await _orchestrator.GetAccount("AVC123");

            //Assert
            Assert.AreEqual(AccountAgreementType.Unknown, result.AccountAgreementType);
        }
    }
}
