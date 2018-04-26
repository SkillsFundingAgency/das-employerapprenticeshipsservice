using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetReceivedTransferConnectionInvitation
    {
        private GetReceivedTransferConnectionInvitationQueryHandler _handler;
        private GetReceivedTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation;
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
                Id = 333333,
                HashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Models.Account.Account
            {
                Id = 222222,
                HashedId = "XYZ987",
                Name = "Receiver"
            };

            _receivedTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
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

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _receivedTransferConnectionInvitation, _rejectedTransferConnectionInvitation };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetReceivedTransferConnectionInvitationQueryHandler(_db.Object, _configurationProvider);

            _query = new GetReceivedTransferConnectionInvitationQuery
            {
                AccountId = _receiverAccount.Id,
                TransferConnectionInvitationId = _receivedTransferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnReceivedTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetReceivedTransferConnectionInvitationResponse>());
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_receivedTransferConnectionInvitation.Id));
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