﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.EmployerAccounts.Models.Transfers;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Hashing;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands
{
    [TestFixture]
    public class SendTransferConnectionInvitationHandlerTests : FluentTest<SendTransferConnectionInvitationHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenISendATransferConnectionInvitation_ThenShouldAddTransferConnectionInvitationToRepository()
        {
            return RunAsync(f => f.Handle(), f => f.TransferConnectionInvitationRepository.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once));
        }
    }

    public class SendTransferConnectionInvitationHandlerTestsFixture : FluentTestFixture
    {
        public SendTransferConnectionInvitationCommandHandler Handler { get; set; }
        public SendTransferConnectionInvitationCommand Command { get; set; }
        public Mock<IEmployerAccountRepository> EmployerAccountRepository { get; set; }
        public Mock<IPublicHashingService> PublicHashingService { get; set; }
        public Mock<ITransferAllowanceService> TransferAllowanceService { get; set; }
        public Mock<ITransferConnectionInvitationRepository> TransferConnectionInvitationRepository { get; set; }
        public Mock<IUserAccountRepository> UserRepository { get; set; }
        public Account ReceiverAccount { get; set; }
        public long? Result { get; set; }
        public Account SenderAccount { get; set; }
        public decimal SenderAccountTransferAllowance { get; set; }
        public User SenderUser { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }

        public SendTransferConnectionInvitationHandlerTestsFixture()
        {
            EmployerAccountRepository = new Mock<IEmployerAccountRepository>();
            PublicHashingService = new Mock<IPublicHashingService>();
            TransferAllowanceService = new Mock<ITransferAllowanceService>();
            TransferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            UserRepository = new Mock<IUserAccountRepository>();

            SetSenderAccount()
                .SetReceiverAccount()
                .SetSenderUser()
                .SetSenderAccountTransferAllowance(1);

            Handler = new SendTransferConnectionInvitationCommandHandler
            (
                EmployerAccountRepository.Object,
                PublicHashingService.Object,
                TransferAllowanceService.Object,
                TransferConnectionInvitationRepository.Object,
                UserRepository.Object
            );

            Command = new SendTransferConnectionInvitationCommand
            {
                AccountId = SenderAccount.Id,
                UserRef = SenderUser.Ref,
                ReceiverAccountPublicHashedId = ReceiverAccount.PublicHashedId
            };
        }

        public SendTransferConnectionInvitationHandlerTestsFixture AddAccount(Account account)
        {
            EmployerAccountRepository.Setup(r => r.GetAccountById(account.Id)).ReturnsAsync(account);
            PublicHashingService.Setup(h => h.DecodeValue(account.PublicHashedId)).Returns(account.Id);

            return this;
        }

        public SendTransferConnectionInvitationHandlerTestsFixture AddInvitationFromSenderToReceiver(TransferConnectionInvitationStatus status)
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

        public SendTransferConnectionInvitationHandlerTestsFixture SetReceiverAccount()
        {
            ReceiverAccount = new Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987",
                Name = "Receiver"
            };

            return AddAccount(ReceiverAccount);
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderAccount()
        {
            SenderAccount = new Account
            {
                Id = 333333,
                PublicHashedId = "ABC123",
                Name = "Sender"
            };

            return AddAccount(SenderAccount);
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderAccountTransferAllowance(decimal remainingTransferAllowance)
        {
            var transferAllowance = new TransferAllowance { RemainingTransferAllowance = remainingTransferAllowance };

            TransferAllowanceService.Setup(s => s.GetTransferAllowance(SenderAccount.Id)).ReturnsAsync(transferAllowance);

            return this;
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderUser()
        {
            SenderUser = new User
            {
                Ref = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            UserRepository
                .Setup(r => r.Get(SenderUser.Id))
                .ReturnsAsync(SenderUser);

            return this;
        }
    }
}