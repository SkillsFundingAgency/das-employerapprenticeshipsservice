//using Moq;
//using NUnit.Framework;
//using SFA.DAS.EmployerAccounts.Interfaces;
//using SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship;
//using SFA.DAS.Encoding;
//using SFA.DAS.Validation;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SFA.DAS.NLog.Logger;
//using SFA.DAS.CommitmentsV2.Types.Dtos;
//using SFA.DAS.EmployerAccounts.Models.Commitments;

//namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSingleDraftApprenticeship
//{
//    public class WhenIGetSingleDraftApprenticeship : QueryBaseTest<GetSingleDraftApprenticeshipRequestHandler, GetSingleDraftApprenticeshipRequest, GetSingleDraftApprenticeshipResponse>
//    {
//        public override GetSingleDraftApprenticeshipRequest Query { get; set; }
//        public override GetSingleDraftApprenticeshipRequestHandler RequestHandler { get; set; }
//        public override Mock<IValidator<GetSingleDraftApprenticeshipRequest>> RequestValidator { get; set; }

//        private Mock<ICommitmentV2Service> _commitmentV2Service;
//        private Mock<IEncodingService> _encodingService;
//        private Mock<ILog> _logger;
//        private long _cohortId;

//        [SetUp]
//        public void Arrange()
//        {
//            SetUp();

//            _cohortId = 123;
//            _logger = new Mock<ILog>();

//            _commitmentV2Service = new Mock<ICommitmentV2Service>();
//            _commitmentV2Service.Setup(m => m.GetDraftApprenticeships(_cohortId)).ReturnsAsync(new List<Apprenticeship> { new Apprenticeship { Id = 4 } });            
//            _encodingService = new Mock<IEncodingService>();
//            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");

//            RequestHandler = new GetSingleDraftApprenticeshipRequestHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object, _encodingService.Object);

//            Query = new GetSingleDraftApprenticeshipRequest
//            {
//                CohortId = _cohortId
//            };
//        }
      
//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
//        {
//            //Act
//            var response = await RequestHandler.Handle(Query);

//            //Assert            
//            Assert.IsNotNull(response.DraftApprenticeshipsResponse);
//            Assert.AreEqual("4_Encoded", response.HashedDraftApprenticeshipId);
//        }


//        [Test]
//        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
//        {
//            //Act
//            await RequestHandler.Handle(Query);

//            //Assert
//            _commitmentV2Service.Verify(x => x.GetDraftApprenticeships(_cohortId), Times.Once);
//        }

//        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
