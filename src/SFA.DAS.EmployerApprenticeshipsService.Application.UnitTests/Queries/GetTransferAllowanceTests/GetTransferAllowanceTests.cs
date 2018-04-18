using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferAllowanceTests
{
    [TestFixture]
    public class GetTransferAllowanceTests : FluentTest<GetTransferAllowanceTestFixtures>
    {
        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldCallDatabase()
        {
            await RunAsync(
                 f => f.WithTransferAllowance(f.TransferAllowance)
                       .WithNoTransferPayments(),
                 f => f.Handle(f.SenderAccountId),
                 f => f.FinancialDatabaseMock.Verify(d => d.SqlQueryAsync<decimal?>(
                    It.Is<string>(q => q.StartsWith("[employer_financial].[GetAccountTransferAllowance]")),
                    f.SenderAccountId,
                    f.LevyDeclarationProviderConfiguration.TransferAllowancePercentage), Times.Once));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnCorrectAllowance()
        {
            await RunAsync(
                     f => f.WithTransferAllowance(f.TransferAllowance)
                           .WithNoTransferPayments(),
                     f => f.Handle(f.SenderAccountId),
                     f => Assert.That(f.Response.TransferAllowance, Is.EqualTo(f.TransferAllowance)));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnAllowanceMinusPayments()
        {
            await RunAsync(
                f => f.WithTransferAllowance(f.TransferAllowance)
                      .WithTransferPayments(),
                f => f.Handle(f.SenderAccountId),
                f => Assert.That(f.Response.TransferAllowance,
                     Is.EqualTo(f.TransferAllowance - f.SenderAccountTransfers.Sum(t => t.Amount))));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnZeroIfAllowanceIsBelowZero()
        {
            await RunAsync(
                f => f.WithTransferAllowance(10)
                    .WithTransferPayments(),
                f => f.Handle(f.SenderAccountId),
                f => Assert.That(f.Response.TransferAllowance,
                    Is.EqualTo(0)));
        }

        [Test]
        public async Task Handle_WhenMakingAnInvalidCall_ShouldThrowException()
        {
            await RunAsync(
                f => f.WithTransferAllowance(f.TransferAllowance),
                f => f.Handle(null),
                f => Assert.IsNotNull(f.HandlerException));
        }

        [Test]
        public async Task Handle_WhenMakingAnInvalidCall_ShouldNotCallDatabase()
        {
            await RunAsync(
                f => f.WithTransferAllowance(f.TransferAllowance),
                f => f.Handle(null),
                f => f.FinancialDatabaseMock.Verify(d => d.SqlQueryAsync<decimal?>(
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<decimal>()), Times.Never));
        }
    }

    public class GetTransferAllowanceTestFixtures
    {
        public long SenderAccountId => 111111;
        public long ReceiverAccountId => 222222;
        public decimal TransferAllowance => 1500.235m;
        public decimal TransferAllowancePercentage => 10;

        public Mock<EmployerFinancialDbContext> FinancialDatabaseMock { get; }
        public EmployerFinancialDbContext FinancialDatabase => FinancialDatabaseMock.Object;
        public LevyDeclarationProviderConfiguration LevyDeclarationProviderConfiguration =>
                    new LevyDeclarationProviderConfiguration
                    { TransferAllowancePercentage = TransferAllowancePercentage };

        public IEnumerable<AccountTransfer> SenderAccountTransfers { get; }

        public GetTransferAllowanceResponse Response { get; set; }

        public Exception HandlerException { get; set; }

        public GetTransferAllowanceTestFixtures()
        {
            FinancialDatabaseMock = new Mock<EmployerFinancialDbContext>();

            SenderAccountTransfers = new[]
            {
                new AccountTransfer
                {
                    SenderAccountId = SenderAccountId,
                    ReceiverAccountId = ReceiverAccountId,
                    Amount = 100.25m
                },
                new AccountTransfer
                {
                    SenderAccountId = SenderAccountId,
                    ReceiverAccountId = ReceiverAccountId,
                    Amount = 250.34m
                },
                new AccountTransfer
                {
                    SenderAccountId = SenderAccountId,
                    ReceiverAccountId = ReceiverAccountId,
                    Amount = 700.345m
                }
            };
        }

        public GetTransferAllowanceTestFixtures WithTransferAllowance(decimal transferAllowance)
        {
            FinancialDatabaseMock.Setup(d => d.SqlQueryAsync<decimal?>
                                (
                                    It.Is<string>(s => s.StartsWith("[employer_financial].[GetAccountTransferAllowance]")),
                                    It.IsAny<long>(),
                                    It.IsAny<decimal>())
                                ).ReturnsAsync(new List<decimal?> { transferAllowance });
            return this;
        }

        public GetTransferAllowanceTestFixtures WithTransferPayments()
        {
            FinancialDatabaseMock.Setup(d => d.SqlQueryAsync<AccountTransfer>
                    (
                        It.Is<string>(s => s.StartsWith("[employer_financial].[GetSenderAccountTransactionsInCurrentFinancialYear]")),
                        It.IsAny<long>())
                ).ReturnsAsync(SenderAccountTransfers.ToList());

            return this;
        }

        public GetTransferAllowanceTestFixtures WithNoTransferPayments()
        {
            FinancialDatabaseMock.Setup(d => d.SqlQueryAsync<AccountTransfer>
                (
                    It.Is<string>(s => s.StartsWith("[employer_financial].[GetSenderAccountTransactionsInCurrentFinancialYear]")),
                    It.IsAny<long>())
            ).ReturnsAsync(new List<AccountTransfer>());

            return this;
        }

        public async Task Handle(long? accountId)
        {
            var query = new GetTransferAllowanceQuery
            {
                AccountId = accountId
            };

            var handler = CreateHandler();

            try
            {
                Response = await handler.Handle(query);
            }
            catch (Exception e)
            {
                HandlerException = e;
            }
        }

        private GetTransferAllowanceQueryHandler CreateHandler()
        {
            return new GetTransferAllowanceQueryHandler(FinancialDatabase, LevyDeclarationProviderConfiguration);
        }
    }
}


