using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.NServiceBus;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.UnitTests.Models
{
    [TestFixture]
    public class AccountTests : FluentTest<AccountTestsFixture>
    {
        [Test]
        public void SendTransferConnectionInvitation_WhenISendATransferConnectionInvitation_ThenShouldReturnTransferConnectionInvitation()
        {
            Run(f => f.SendTransferConnectionInvitation(), (f, i) => i.Should().NotBeNull());
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenSenderIsTheReceiver_ThenShouldThrowException()
        {
            Run(f => f.SetSenderAccountToReceiverAccount(), f => f.SendTransferConnectionInvitation(), (f, a) => a.ShouldThrow<Exception>()
                .WithMessage("Requires transfer connection invitation sender is not the receiver"));
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenMinTransferAllowanceIsNotAvailable_ThenShouldThrowException()
        {
            Run(f => f.SetSenderAccountTransferAllowance(0), f => f.SendTransferConnectionInvitation(), (f, a) => a.ShouldThrow<Exception>()
                .WithMessage("Requires min transfer allowance is available"));
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenSenderIsAReceiver_ThenShouldThrowException()
        {
            Run(f => f.SetSenderAccountWithReceivedTransferConnection(), f => f.SendTransferConnectionInvitation(), (f, a) => a.ShouldThrow<Exception>()
                .WithMessage("Requires transfer connection invitation sender is not a receiver"));
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenReceiverIsASender_ThenShouldThrowException()
        {
            Run(f => f.SetReceiverAccountWithSentTransferConnection(), f => f.SendTransferConnectionInvitation(), (f, a) => a.ShouldThrow<Exception>()
                .WithMessage("Requires transfer connection invitation receiver is not a sender"));
        }
    }

    public class AccountTestsFixture : FluentTestFixture
    {
        public IUnitOfWorkContext UnitOfWorkContext { get; set; } = new UnitOfWorkContext();
        public Account SenderAccount { get; set; }
        public Account ReceiverAccount { get; set; }
        public User SenderUser { get; set; }
        public decimal SenderAccountTransferAllowance { get; set; }

        public AccountTestsFixture()
        {
            SetSenderAccount(new AccountBuilder().WithId(1).Build())
                .SetReceiverAccount(new AccountBuilder().WithId(2).Build())
                .SetSenderUser(new UserBuilder().Build())
                .SetSenderAccountTransferAllowance(1);
        }

        public TransferConnectionInvitation SendTransferConnectionInvitation()
        {
            return SenderAccount.SendTransferConnectionInvitation(ReceiverAccount, SenderUser, SenderAccountTransferAllowance);
        }

        public AccountTestsFixture SetReceiverAccount(Account account)
        {
            ReceiverAccount = account;

            return this;
        }

        public AccountTestsFixture SetSenderAccountTransferAllowance(decimal transferAllowance)
        {
            SenderAccountTransferAllowance = transferAllowance;

            return this;
        }

        public AccountTestsFixture SetSenderUser(User user)
        {
            SenderUser = user;

            return this;
        }

        public AccountTestsFixture SetSenderAccount(Account account)
        {
            SenderAccount = account;

            return this;
        }

        public AccountTestsFixture SetReceiverAccountWithSentTransferConnection()
        {
            ReceiverAccount = new AccountBuilder()
                .WithSentTransferConnectionInvitation(new TransferConnectionInvitationBuilder()
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build())
                .Build();

            return this;
        }

        public AccountTestsFixture SetSenderAccountWithReceivedTransferConnection()
        {
            SenderAccount = new AccountBuilder()
                .WithReceivedTransferConnectionInvitation(new TransferConnectionInvitationBuilder()
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build())
                .Build();

            return this;
        }

        public AccountTestsFixture SetSenderAccountWithReceivedTransferConnectionFromReceiverAccount()
        {
            SenderAccount = new AccountBuilder()
                .WithReceivedTransferConnectionInvitation(new TransferConnectionInvitationBuilder()
                    .WithSenderAccount(ReceiverAccount)
                    .WithStatus(TransferConnectionInvitationStatus.Approved)
                    .Build())
                .Build();

            return this;
        }

        public AccountTestsFixture SetSenderAccountWithSentTransferConnection()
        {
            SenderAccount = new AccountBuilder()
                .WithSentTransferConnectionInvitation(new TransferConnectionInvitationBuilder()
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build())
                .Build();

            return this;
        }

        public AccountTestsFixture SetSenderAccountWithSentTransferConnectionToReceiverAccount()
        {
            SenderAccount = new AccountBuilder()
                .WithSentTransferConnectionInvitation(new TransferConnectionInvitationBuilder()
                    .WithReceiverAccount(ReceiverAccount)
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build())
                .Build();

            return this;
        }

        public AccountTestsFixture SetSenderAccountToReceiverAccount()
        {
            SenderAccount = ReceiverAccount = new AccountBuilder().WithId(1).Build();

            return this;
        }
    }
}