using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnections;
using SFA.DAS.EmployerFinance.TestCommon.Builders;
using SFA.DAS.EmployerFinance.TestCommon.Helpers;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransferConnectionsTests
{
    [TestFixture]
    public class WhenIGetTransferConnections
    {
        private GetTransferConnectionsQueryHandler _handler;
        private GetTransferConnectionsQuery _query;
        private GetTransferConnectionsResponse _response;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private IMapper _mapper;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _approvedTransferConnectionAtoB;
        private TransferConnectionInvitation _rejectedTransferConnectionAtoC;
        private TransferConnectionInvitation _approvedTransferConnectionAtoC;
        private TransferConnectionInvitation _approvedTransferConnectionBtoC;
        private Account _accountA;
        private Account _accountB;
        private Account _accountC;

        [SetUp]
        public void Arrange()
        {
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();

            _accountA = new Account
            {
                Id = 111111,
                Name = "Account A",
                HashingService = new TestHashingService(),
                PublicHashingService = new TestPublicHashingService()
            };

            _accountB = new Account
            {
                Id = 222222,
                Name = "Account B",
                HashingService = new TestHashingService(),
                PublicHashingService = new TestPublicHashingService()
            };

            _accountC = new Account
            {
                Id = 333333,
                Name = "Account C",
                HashingService = new TestHashingService(),
                PublicHashingService = new TestPublicHashingService()
            };

            _approvedTransferConnectionAtoB = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_accountA)
                .WithReceiverAccount(_accountB)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _rejectedTransferConnectionAtoC = new TransferConnectionInvitationBuilder()
                .WithId(222222)
                .WithSenderAccount(_accountA)
                .WithReceiverAccount(_accountC)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();

            _approvedTransferConnectionAtoC = new TransferConnectionInvitationBuilder()
                .WithId(333333)
                .WithSenderAccount(_accountA)
                .WithReceiverAccount(_accountC)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _approvedTransferConnectionBtoC = new TransferConnectionInvitationBuilder()
                .WithId(444444)
                .WithSenderAccount(_accountB)
                .WithReceiverAccount(_accountC)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _transferConnectionInvitations = new List<TransferConnectionInvitation>
            {
                _approvedTransferConnectionAtoB,
                _rejectedTransferConnectionAtoC,
                _approvedTransferConnectionAtoC,
                _approvedTransferConnectionBtoC 
            };

            _transferConnectionInvitationRepository
                .Setup(s => s.GetByReceiver(_accountC.Id, TransferConnectionInvitationStatus.Approved))
                .ReturnsAsync(new List<TransferConnectionInvitation>
            {
                _approvedTransferConnectionAtoC,
                _approvedTransferConnectionBtoC
            });

            _mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
            }));

            _handler = new GetTransferConnectionsQueryHandler(_transferConnectionInvitationRepository.Object, _mapper);

            _query = new GetTransferConnectionsQuery
            {
                AccountId = _accountC.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferConnections.Count(), Is.EqualTo(2));

            var transferConnectionInvitation = _response.TransferConnections.ElementAt(0);

            Assert.That(transferConnectionInvitation.FundingEmployerAccountId, Is.EqualTo(_accountA.Id));
            Assert.That(transferConnectionInvitation.FundingEmployerHashedAccountId, Is.EqualTo(_accountA.HashedId));
            Assert.That(transferConnectionInvitation.FundingEmployerPublicHashedAccountId, Is.EqualTo(_accountA.PublicHashedId));
            Assert.That(transferConnectionInvitation.FundingEmployerAccountName, Is.EqualTo(_accountA.Name));

            var transferConnectionInvitation1 = _response.TransferConnections.ElementAt(1);

            Assert.That(transferConnectionInvitation1.FundingEmployerAccountId, Is.EqualTo(_accountB.Id));
            Assert.That(transferConnectionInvitation1.FundingEmployerHashedAccountId, Is.EqualTo(_accountB.HashedId));
            Assert.That(transferConnectionInvitation1.FundingEmployerPublicHashedAccountId, Is.EqualTo(_accountB.PublicHashedId));
            Assert.That(transferConnectionInvitation1.FundingEmployerAccountName, Is.EqualTo(_accountB.Name));
        }
    }
}