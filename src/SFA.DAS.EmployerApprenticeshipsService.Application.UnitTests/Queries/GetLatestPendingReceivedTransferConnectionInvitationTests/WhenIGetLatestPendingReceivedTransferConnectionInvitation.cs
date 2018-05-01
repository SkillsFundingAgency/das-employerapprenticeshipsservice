using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLatestPendingReceivedTransferConnectionInvitationTests
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
        public Mock<EmployerAccountDbContext> Db { get; set; }
        public List<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation1 { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation2 { get; set; }
        public TransferConnectionInvitation ApprovedReceivedTransferConnectionInvitation { get; set; }
        public Domain.Models.Account.Account SenderAccount { get; set; }
        public Domain.Models.Account.Account ReceiverAccount { get; set; }

        public LatestPendingTransferConnectionInvitationFixture()
        {
            var configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            Db = new Mock<EmployerAccountDbContext>();

            SenderAccount = new Domain.Models.Account.Account
            {
                Id = 1,
                Name = "Sender"
            };

            ReceiverAccount = new Domain.Models.Account.Account
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
                    .WithSenderAccount(new Domain.Models.Account.Account())
                    .WithReceiverAccount(new Domain.Models.Account.Account())
                    .WithStatus(TransferConnectionInvitationStatus.Pending)
                    .Build()
            };

            Db.Setup(d => d.TransferConnectionInvitations).Returns(new DbSetStub<TransferConnectionInvitation>(TransferConnectionInvitations));

            Handler = new GetLatestPendingReceivedTransferConnectionInvitationQueryHandler(Db.Object, configurationProvider);
            Query = new GetLatestPendingReceivedTransferConnectionInvitationQuery { AccountId = ReceiverAccount.Id };
        }

        public async Task Handle()
        {
            Response = await Handler.Handle(Query);
        }
    }
}
