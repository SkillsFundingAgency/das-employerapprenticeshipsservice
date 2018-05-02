using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSentTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetSentTransferConnectionInvitation
    {
        private GetSentTransferConnectionInvitationQueryHandler _handler;
        private GetSentTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _sentTransferConnectionInvitation;
        private TransferConnectionInvitation _rejectedTransferConnectionInvitation;
        private Domain.Models.Account.Account _senderAccount;
        private Domain.Models.Account.Account _receiverAccount;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _senderAccount = new Domain.Models.Account.Account
            {
                Id = 444444,
                HashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Models.Account.Account
            {
                Id = 333333,
                HashedId = "XYZ987",
                Name = "Receiver"
            };
            
            _sentTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(222222)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .Build();
            
            _rejectedTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _sentTransferConnectionInvitation, _rejectedTransferConnectionInvitation };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetSentTransferConnectionInvitationQueryHandler(_db.Object, _configurationProvider);

            _query = new GetSentTransferConnectionInvitationQuery
            {
                AccountId = _senderAccount.Id,
                TransferConnectionInvitationId = _sentTransferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnSentTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetSentTransferConnectionInvitationResponse>());
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_sentTransferConnectionInvitation.Id));
        }

        [Test]
        public async Task ThenShouldReturnNullIfTransferConnectionInvitationIsNull()
        {
            _transferConnectionInvitations.Clear();

            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Null);
        }
    }
}