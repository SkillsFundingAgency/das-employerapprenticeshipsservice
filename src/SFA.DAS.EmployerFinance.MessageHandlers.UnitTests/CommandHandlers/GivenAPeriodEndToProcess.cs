using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MediatR;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    public class GivenAPeriodEndToProcess
    {
        private ProcessPeriodEndPaymentsCommandHandler _sut;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILog> _loggerMock;
        private TestableMessageHandlerContext _messageHandlerContext;
        private IFixture Fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _messageHandlerContext = new TestableMessageHandlerContext();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILog>();

            Fixture.Customize<Account>(x => x.Without(s => s.AccountLegalEntities));
            Fixture.Customize(new AutoMoqCustomization());

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { Fixture.Create<Account>() } });

            _sut = new ProcessPeriodEndPaymentsCommandHandler(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single new period end to process")]
        [TestCase(5, Description = "Multiple new period ends to process")]
        public async Task WhenThereAreNewPeriodsAndMultipleAccountsExistProccesPaymentsForPeriodForEachAccount(int amountOfNewPeriodEnds)
        {
            // Arrange
            var accounts = Fixture.CreateMany<Account>(3).ToList();
            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = accounts });
            var processPeriodEndCommand = new ProcessPeriodEndPaymentsCommand { PeriodEndRef = "1819-R01" };

            // Act
            await _sut.Handle(processPeriodEndCommand, _messageHandlerContext);

            // Assert
            VerifyProcessAccountPaymentsCommandSentForEachAccountForEachNewPeriodEnd(processPeriodEndCommand.PeriodEndRef, accounts);
        }

        private void VerifyProcessAccountPaymentsCommandSentForEachAccountForEachNewPeriodEnd(string periodEnd, List<Account> accounts)
        {
            var commandsSent = _messageHandlerContext
                .SentMessages
                .Where(sm => sm.Message.GetType() == typeof(ImportAccountPaymentsCommand))
                .Select(sm => sm.Message as ImportAccountPaymentsCommand);

            foreach (var account in accounts)
            {
                commandsSent.Should().ContainSingle(x => x.PeriodEndRef == periodEnd && x.AccountId == account.Id);
            }
        }
    }
}
