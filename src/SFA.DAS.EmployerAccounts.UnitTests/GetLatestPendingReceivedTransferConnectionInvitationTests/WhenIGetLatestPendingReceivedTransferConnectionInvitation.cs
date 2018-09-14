using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.EmployerAccounts.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.UnitTests.GetLatestPendingReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetLatestPendingReceivedTransferConnectionInvitation : FluentTest<LatestPendingTransferConnectionInvitationFixture>
    {
        [Test]
        public void Handle_WhenIGetLatestPendingReceivedTransferConnectionInvitation_ThenShouldReturnResponse()
        {
            RunAsync(f => f.Handle(), f => f.Response.Should().NotBeNull()
                .And.Match<GetLatestPendingReceivedTransferConnectionInvitationResponse>(r =>
                    r.TransferConnectionInvitation != null &&
                    r.TransferConnectionInvitation.Id == f.PendingReceivedTransferConnectionInvitation2.Id &&
                    r.TransferConnectionInvitation.SenderAccount.Id == f.SenderAccount.Id &&
                    r.TransferConnectionInvitation.ReceiverAccount.Id == f.ReceiverAccount.Id &&
                    r.TransferConnectionInvitation.Status == f.PendingReceivedTransferConnectionInvitation2.Status &&
                    r.TransferConnectionInvitation.CreatedDate == f.PendingReceivedTransferConnectionInvitation2.CreatedDate));
        }
    }

    public class LatestPendingTransferConnectionInvitationFixture : FluentTestFixture
    {
        public GetLatestPendingReceivedTransferConnectionInvitationResponse Response { get; set; }
        public GetLatestPendingReceivedTransferConnectionInvitationQueryHandler Handler { get; set; }
        public GetLatestPendingReceivedTransferConnectionInvitationQuery Query { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public List<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation1 { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation2 { get; set; }
        public TransferConnectionInvitation ApprovedReceivedTransferConnectionInvitation { get; set; }
        public Account SenderAccount { get; set; }
        public Account ReceiverAccount { get; set; }

        public LatestPendingTransferConnectionInvitationFixture()
        {
            var configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            Db = new Mock<EmployerAccountsDbContext>();

            SenderAccount = new Account
            {
                Id = 1,
                Name = "Sender"
            };

            ReceiverAccount = new Account
            {
                Id = 2,
                Name = "Receiver"
            };

            PendingReceivedTransferConnectionInvitation1 = new TransferConnectionInvitationBuilder()
                .WithId(1)
                .WithSenderAccount(SenderAccount)
                .WithReceiverAccount(ReceiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            PendingReceivedTransferConnectionInvitation2 = new TransferConnectionInvitationBuilder()
                .WithId(2)
                .WithSenderAccount(SenderAccount)
                .WithReceiverAccount(ReceiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow)
                .Build();

            ApprovedReceivedTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithSenderAccount(SenderAccount)
                .WithReceiverAccount(ReceiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            TransferConnectionInvitations = new List<TransferConnectionInvitation>
            {
                PendingReceivedTransferConnectionInvitation1,
                ApprovedReceivedTransferConnectionInvitation,
                new TransferConnectionInvitationBuilder()
                    .WithSenderAccount(new Account())
                    .WithReceiverAccount(new Account())
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build()
            };

            Db.Setup(d => d.TransferConnectionInvitations).Returns(new DbSetStub<TransferConnectionInvitation>(TransferConnectionInvitations));

            Handler = new GetLatestPendingReceivedTransferConnectionInvitationQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), configurationProvider);
            Query = new GetLatestPendingReceivedTransferConnectionInvitationQuery { AccountId = ReceiverAccount.Id };
        }

        public async Task Handle()
        {
            Response = await Handler.Handle(Query);
        }
    }
}
