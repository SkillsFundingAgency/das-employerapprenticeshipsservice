using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetSentTransferConnectionInvitation
    {
        private GetSentTransferConnectionInvitationQueryHandler _handler;
        private GetSentTransferConnectionInvitationQuery _query;

        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private Mapper _mapper;

        [SetUp]
        public void Arrange()
        {
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();

            _mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            }));

            _handler = new GetSentTransferConnectionInvitationQueryHandler(_transferConnectionInvitationRepository.Object, _mapper);

            _query = new GetSentTransferConnectionInvitationQuery
            {
                AccountId = 12345,
                TransferConnectionInvitationId = 1
            };
        }

        [Test]
        public async Task ThenShouldCallRepository()
        {
            var response = await _handler.Handle(_query);
            _transferConnectionInvitationRepository.Verify(v => v.GetBySender(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<TransferConnectionInvitationStatus>()), Times.Once);
        }
    }
}