using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MarkerInterfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.EmployerFinance.TestCommon.Builders;
using SFA.DAS.EmployerFinance.TestCommon.Helpers;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands
{
    [TestFixture]
    public class WhenIHandleSendTransferConnectionInvitationCommand : FluentTest<WhenIHandleSendTransferConnectionInvitationCommandTestFixture>
    {
        [Test]
        public Task Handle_SendTransferConnectionInvitationCommand_ThenShouldAddTransferConnectionInvitationToRepository()
        {
            return RunAsync(f => f.Handle(), f => f.TransferConnectionInvitationRepository.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once));
        }
    }

    public class WhenIHandleSendTransferConnectionInvitationCommandTestFixture : FluentTestFixture
    {
        public SendTransferConnectionInvitationCommandHandler Handler { get; set; }
        public SendTransferConnectionInvitationCommand Command { get; set; }
        
        public Mock<IEmployerAccountRepository> EmployerAccountRepository { get; set; }
        public Mock<ITransferConnectionInvitationRepository> TransferConnectionInvitationRepository { get; set; }
        public Mock<ITransferRepository> TransferRepository { get; set; }
        public Mock<IUserAccountRepository> UserRepository { get; set; }
        public Mock<IPublicHashingService> PublicHashingService { get; set; }
        public EmployerFinanceConfiguration EmployerFinanceConfiguration { get; set; }

        public Account ReceiverAccount { get; set; }
        public long? Result { get; set; }
        public Account SenderAccount { get; set; }
        public decimal SenderAccountTransferAllowance { get; set; }
        public User SenderUser { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
        public IUnitOfWorkContext UnitOfWorkContext { get; }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture()
        {
            EmployerAccountRepository = new Mock<IEmployerAccountRepository>();
            TransferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            TransferRepository = new Mock<ITransferRepository>();
            UserRepository = new Mock<IUserAccountRepository>();
            PublicHashingService = new Mock<IPublicHashingService>();
            EmployerFinanceConfiguration = new EmployerFinanceConfiguration
            {
                TransferAllowancePercentage = 1
            };

            SetSenderAccount()
                .SetReceiverAccount()
                .SetSenderUser()
                .SetSenderAccountTransferAllowance(1);

            Handler = new SendTransferConnectionInvitationCommandHandler
            (
                EmployerAccountRepository.Object,
                TransferConnectionInvitationRepository.Object,
                TransferRepository.Object,
                UserRepository.Object,
                PublicHashingService.Object,
                EmployerFinanceConfiguration
            );

            Command = new SendTransferConnectionInvitationCommand
            {
                AccountId = SenderAccount.Id,
                UserRef = SenderUser.Ref,
                ReceiverAccountPublicHashedId = ReceiverAccount.PublicHashedId
            };

            UnitOfWorkContext = new UnitOfWorkContext();
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture AddAccount(Account account)
        {
            EmployerAccountRepository.Setup(r => r.Get(account.Id)).ReturnsAsync(account);
            PublicHashingService.Setup(h => h.DecodeValue(account.PublicHashedId)).Returns(account.Id);

            return this;
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture AddInvitationFromSenderToReceiver(TransferConnectionInvitationStatus status)
        {
            SenderAccount.SentTransferConnectionInvitations.Add(
                new TransferConnectionInvitationBuilder()
                    .WithReceiverAccount(ReceiverAccount)
                    .WithStatus(status)
                    .Build());

            return this;
        }

        public async Task Handle()
        {
            Result = await Handler.Handle(Command);
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture SetReceiverAccount()
        {
            ReceiverAccount = new Account
            {
                Id = 222222,
                Name = "Receiver",
                HashingService = new TestHashingService(),
                PublicHashingService = new TestPublicHashingService()
            };

            return AddAccount(ReceiverAccount);
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture SetSenderAccount()
        {
            SenderAccount = new Account
            {
                Id = 333333,
                Name = "Sender",
                HashingService = new TestHashingService(),
                PublicHashingService = new TestPublicHashingService()
            };

            return AddAccount(SenderAccount);
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture SetSenderAccountTransferAllowance(decimal remainingTransferAllowance)
        {
            var transferAllowance = new TransferAllowance { RemainingTransferAllowance = remainingTransferAllowance };

            TransferRepository.Setup(s => s.GetTransferAllowance(SenderAccount.Id, It.IsAny<decimal>())).ReturnsAsync(transferAllowance);

            return this;
        }

        public WhenIHandleSendTransferConnectionInvitationCommandTestFixture SetSenderUser()
        {
            SenderUser = new User
            {
                Ref = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            UserRepository
                .Setup(r => r.Get(SenderUser.Ref))
                .ReturnsAsync(SenderUser);

            return this;
        }
    }
}