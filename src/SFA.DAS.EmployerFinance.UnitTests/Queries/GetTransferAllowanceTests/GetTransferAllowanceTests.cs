using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Queries.GetTransferAllowance;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransferAllowanceTests
{
    [TestFixture]
    public class GetTransferAllowanceTests : FluentTest<GetTransferAllowanceTestsFixture>
    {
        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldCallRepository()
        {
            await RunAsync(
                 f => f.WithTransferAllowance(f.TransferAllowance),
                 f => f.Handle(f.SenderAccountId),
                 f => f.TransferRepository.Verify(v => v.GetTransferAllowance(
                    f.SenderAccountId,
                    f.EmployerFinanceConfig.TransferAllowancePercentage), Times.Once));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnCorrectAllowance()
        {
            await RunAsync(
                     f => f.WithTransferAllowance(f.TransferAllowance),
                     f => f.Handle(f.SenderAccountId),
                     f => Assert.That(f.Response.TransferAllowance.RemainingTransferAllowance, Is.EqualTo(f.TransferAllowance)));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnAllowanceMinusPayments()
        {
            await RunAsync(
                f => f.WithTransferAllowance(f.TransferAllowance),
                f => f.Handle(f.SenderAccountId),
                f => Assert.That(f.Response.TransferAllowance.RemainingTransferAllowance,
                     Is.EqualTo(f.TransferAllowance)));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnZeroIfAllowanceIsBelowZero()
        {
            await RunAsync(
                f => f.WithTransferAllowance(-10),
                f => f.Handle(f.SenderAccountId),
                f => Assert.That(f.Response.TransferAllowance.RemainingTransferAllowance,
                    Is.EqualTo(0)));
        }

        [Test]
        public async Task Handle_WhenMakingAValidCall_ShouldReturnCorrectTransferAllowancePercentage()
        {
            await RunAsync(
                     f => f.WithTransferAllowance(f.TransferAllowance),
                     f => f.Handle(f.SenderAccountId),
                     f => Assert.That(f.Response.TransferAllowancePercentage, Is.EqualTo(25m)));
        }
    }

    public class GetTransferAllowanceTestsFixture : FluentTestFixture
    {
        public long SenderAccountId => 111111;
        public long ReceiverAccountId => 222222;
        public decimal TransferAllowance => 1500.235m;
        public decimal TransferAllowancePercentage => 25;

        public Mock<ITransferRepository> TransferRepository { get; set; }
        public EmployerFinanceConfiguration EmployerFinanceConfig { get; set; }
            
        public IEnumerable<AccountTransfer> SenderAccountTransfers { get; }

        public GetTransferAllowanceResponse Response { get; set; }

        public Exception HandlerException { get; set; }

        public GetTransferAllowanceTestsFixture()
        {
            TransferRepository = new Mock<ITransferRepository>();
            EmployerFinanceConfig = new EmployerFinanceConfiguration { TransferAllowancePercentage = TransferAllowancePercentage };

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

        public GetTransferAllowanceTestsFixture WithTransferAllowance(decimal remainingTransferAllowance)
        {
            var transferAllowance = new TransferAllowance
            {
                RemainingTransferAllowance = remainingTransferAllowance
            };

            TransferRepository
                .Setup(s => s.GetTransferAllowance(It.IsAny<long>(), It.IsAny<decimal>()))
                .ReturnsAsync(transferAllowance);

            return this;
        }

        public async Task Handle(long accountId)
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
            return new GetTransferAllowanceQueryHandler(TransferRepository.Object, EmployerFinanceConfig);
        }
    }
}


