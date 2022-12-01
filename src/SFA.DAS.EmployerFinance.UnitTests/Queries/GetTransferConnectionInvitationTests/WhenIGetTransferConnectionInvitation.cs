using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitation
    {
        private GetTransferConnectionInvitationQueryHandler _handler;
        private GetTransferConnectionInvitationQuery _query;


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

            _handler = new GetTransferConnectionInvitationQueryHandler(_transferConnectionInvitationRepository.Object, _mapper);

            _query = new GetTransferConnectionInvitationQuery
            {
                AccountId = 12345,
                TransferConnectionInvitationId = 123
            };
        }

        [Test]
        public async Task ThenShouldCallRepository()
        {
            var response = await _handler.Handle(_query);
            _transferConnectionInvitationRepository.Verify(v => v.GetBySenderOrReceiver(It.IsAny<int>(), It.IsAny<long>()), Times.Once);
        }
    }
}