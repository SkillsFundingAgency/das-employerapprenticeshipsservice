using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnectionInvitation
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
        public Mock<IUserRepository> UserRepository { get; set; }
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public long? Result { get; set; }
        public Domain.Data.Entities.Account.Account SenderAccount { get; set; }
        public decimal SenderAccountTransferAllowance { get; set; }
        public User SenderUser { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }

        public SendTransferConnectionInvitationHandlerTestsFixture()
        {
            EmployerAccountRepository = new Mock<IEmployerAccountRepository>();
            PublicHashingService = new Mock<IPublicHashingService>();
            TransferAllowanceService = new Mock<ITransferAllowanceService>();
            TransferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            UserRepository = new Mock<IUserRepository>();
            
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
                UserId = SenderUser.Id,
                ReceiverAccountPublicHashedId = ReceiverAccount.PublicHashedId
            };
        }

        public SendTransferConnectionInvitationHandlerTestsFixture AddAccount(Domain.Data.Entities.Account.Account account)
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
            ReceiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987",
                Name = "Receiver"
            };

            return AddAccount(ReceiverAccount);
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderAccount()
        {
            SenderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                PublicHashedId = "ABC123",
                Name = "Sender"
            };

            return AddAccount(SenderAccount);
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderAccountTransferAllowance(decimal transferAllowance)
        {
            TransferAllowanceService.Setup(s => s.GetTransferAllowance(SenderAccount.Id)).ReturnsAsync(transferAllowance);

            return this;
        }

        public SendTransferConnectionInvitationHandlerTestsFixture SetSenderUser()
        {
            SenderUser = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            UserRepository
                .Setup(r => r.GetUserById(SenderUser.Id))
                .ReturnsAsync(SenderUser);

            return this;
        }
    }
}