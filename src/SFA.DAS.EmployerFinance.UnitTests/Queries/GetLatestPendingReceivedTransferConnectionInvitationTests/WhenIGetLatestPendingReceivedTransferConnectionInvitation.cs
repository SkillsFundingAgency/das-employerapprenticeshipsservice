using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetLatestPendingReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetLatestPendingReceivedTransferConnectionInvitation : FluentTest<LatestPendingTransferConnectionInvitationFixture>
    {
        [Test]
        public void Handle_WhenIGetLatestPendingReceivedTransferConnectionInvitation_ThenShouldCallRepository()
        {
            RunAsync(f => f.Handle(), f => f.VerifyGetLatestByReceiverIsCalled());
        }
    }

    public class LatestPendingTransferConnectionInvitationFixture : FluentTestFixture
    {
        public GetLatestPendingReceivedTransferConnectionInvitationQueryHandler Handler { get; set; }
        public GetLatestPendingReceivedTransferConnectionInvitationQuery Query { get; set; }
        
        public Mock<ITransferConnectionInvitationRepository> TransferConnectionInvitationRepository;
        public Mapper _mapper;
        
        public long ReceiverAccountId { get; set; }

        public LatestPendingTransferConnectionInvitationFixture()
        {
            _mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            }));

            TransferConnectionInvitationRepository
                .Setup(s => s.GetLatestByReceiver(ReceiverAccountId, TransferConnectionInvitationStatus.Pending))
                .ReturnsAsync(new TransferConnectionInvitation());

            Handler = new GetLatestPendingReceivedTransferConnectionInvitationQueryHandler(TransferConnectionInvitationRepository.Object, _mapper);
            Query = new GetLatestPendingReceivedTransferConnectionInvitationQuery { AccountId = 12345 };
        }

        public async Task Handle()
        {
            await Handler.Handle(Query);
        }

        public void VerifyGetLatestByReceiverIsCalled()
        {
            TransferConnectionInvitationRepository
                .Verify(s => s.GetLatestByReceiver(ReceiverAccountId, TransferConnectionInvitationStatus.Pending), Times.Once);
        }
    }
}
