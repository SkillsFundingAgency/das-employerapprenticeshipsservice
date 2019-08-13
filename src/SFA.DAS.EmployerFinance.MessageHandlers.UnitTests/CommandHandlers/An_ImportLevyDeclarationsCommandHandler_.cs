using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class An_ImportLevyDeclarationsCommandHandler_
    {
        private ImportLevyDeclarationsCommandHandler _handler;
        private Mock<IPayeRepository> _payeRepository;
        private List<Account> _accounts;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
                fixture
                .Register<AccountLegalEntity>(() => new AccountLegalEntity());

            _accounts
                =
                new List<Account>(
                fixture
                    .CreateMany<Account>(
                        new Random()
                            .Next(
                                1,
                                15)));

            var accountsRepository = new Mock<IEmployerAccountRepository>();

            accountsRepository
                .Setup(
                    m => m.GetAllAccounts())
                .ReturnsAsync(_accounts);

            _payeRepository
                =
                new Mock<IPayeRepository>();

            _handler
                =
                new ImportLevyDeclarationsCommandHandler(
                    accountsRepository.Object,
                    Mock.Of<ILog>(),
                    _payeRepository.Object);
        }

        [Test]
        public async Task Only_Processes_PAYE_Schemes_Added_Using_GovernmentGateway()
        {
            _handler
                .Handle(
                    new ImportLevyDeclarationsCommand(),
                    Mock.Of<IMessageHandlerContext>());

            _payeRepository
                .Verify(
                    m =>
                        m.GetGovernmentGatewayOnlySchemesByEmployerId(
                            It.Is<long>(accountId => AccountIdInRepositoryResults(accountId))),
                    Times.Exactly(_accounts.Count));
        }

        private bool AccountIdInRepositoryResults(long accountId)
        {
            return
                _accounts
                    .Any(acc => acc.Id == accountId);
        }
    }
}