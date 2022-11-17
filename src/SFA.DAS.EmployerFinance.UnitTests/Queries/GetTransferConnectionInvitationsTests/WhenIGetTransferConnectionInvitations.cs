using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransferConnectionInvitationsTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitations
    {
        private GetTransferConnectionInvitationsQueryHandler _handler;
        private GetTransferConnectionInvitationsQuery _query;

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

            _handler = new GetTransferConnectionInvitationsQueryHandler(_transferConnectionInvitationRepository.Object, _mapper);

            _query = new GetTransferConnectionInvitationsQuery
            {
                AccountId = 12345
            };
        }

        [Test]
        public async Task ThenShouldCallRepository()
        {
            var response = await _handler.Handle(_query);
            _transferConnectionInvitationRepository.Verify(v => v.GetBySenderOrReceiver(It.IsAny<long>()), Times.Once);
        }
    }
}