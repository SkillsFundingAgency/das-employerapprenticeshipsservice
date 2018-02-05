using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLatestOutstandingTransferInvitationTests
{
    public class WhenIGetTheLatestTransfer : QueryBaseTest<GetLatestOutstandingTransferInvitationHandler, GetLatestOutstandingTransferInvitationQuery, GetLatestOutstandingTransferInvitationResponse>
    {
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private Mock<IHashingService> _hashingService;
        private Mock<IMembershipRepository> _membershipRepository;
        private TransferConnectionInvitation _expectedTransferConnectionInvitation;
        private int _expectedAccountId = 1882;

        public override GetLatestOutstandingTransferInvitationQuery Query { get; set; }
        public override GetLatestOutstandingTransferInvitationHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetLatestOutstandingTransferInvitationQuery>> RequestValidator { get; set; }


        [SetUp]
        public void Arrange()
        {
            SetUp();
            const string expectedHashedAccountId = "AABBCC";

            _expectedTransferConnectionInvitation = new TransferConnectionInvitation();
            _hashingService = new Mock<IHashingService>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();

            var currentUser = new CurrentUser
            {
                ExternalUserId = Guid.NewGuid().ToString()
            };

            Query = new GetLatestOutstandingTransferInvitationQuery
            {
                ReceiverAccountHashedId = expectedHashedAccountId
            };

            _hashingService
                .Setup(m => m.DecodeValue(Query.ReceiverAccountHashedId))
                .Returns(_expectedAccountId);

            _membershipRepository
                .Setup(x => x.GetCaller(Query.ReceiverAccountHashedId, currentUser.ExternalUserId))
                .ReturnsAsync(new MembershipView());

            _transferConnectionInvitationRepository
                .Setup(x => x.GetLatestOutstandingTransferConnectionInvitation(It.IsAny<long>()))
                .ReturnsAsync(_expectedTransferConnectionInvitation);

            RequestHandler = new GetLatestOutstandingTransferInvitationHandler(
                                            currentUser, 
                                            _hashingService.Object, 
                                            _membershipRepository.Object, 
                                            _transferConnectionInvitationRepository.Object,
                                            RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            // Arrange
            EnsureMessageIsValid();

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _transferConnectionInvitationRepository
                .Verify(x => x.GetLatestOutstandingTransferConnectionInvitation(_expectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            // Arrange
            EnsureMessageIsValid();

            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(_expectedTransferConnectionInvitation, response.TransferConnectionInvitation);
        }

        [Test]
        public async Task TheResponseIsNullIfThereAreNoOutstandingConnectionRequests()
        {
            // Arrange
            EnsureMessageIsValid();
            _transferConnectionInvitationRepository.Reset();
            _transferConnectionInvitationRepository
                .Setup(x => x.GetLatestOutstandingTransferConnectionInvitation(It.IsAny<long>()))
                .ReturnsAsync(null);

            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNull(response.TransferConnectionInvitation);
        }

        private void EnsureMessageIsValid()
        {

            RequestValidator
                .Setup(x => x.Validate(It.IsAny<GetLatestOutstandingTransferInvitationQuery>()))
                .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
        }
    }
}
