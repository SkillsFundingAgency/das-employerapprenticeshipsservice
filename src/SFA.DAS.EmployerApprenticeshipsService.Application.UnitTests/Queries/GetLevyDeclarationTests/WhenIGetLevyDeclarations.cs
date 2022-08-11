//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
//using SFA.DAS.Validation;
//using SFA.DAS.EAS.Domain.Data.Repositories;
//using SFA.DAS.EAS.Domain.Models.Levy;
//using SFA.DAS.EAS.TestCommon.ObjectMothers;
//using SFA.DAS.HashingService;

//namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLevyDeclarationTests
//{
//    public class WhenIGetLevyDeclarations : QueryBaseTest<GetLevyDeclarationQueryHandler,GetLevyDeclarationRequest, GetLevyDeclarationResponse>
//    {
//        private Mock<IDasLevyRepository> _repository;
//        private Mock<IHashingService> _hashingService;
//        public override GetLevyDeclarationRequest Query { get; set; }
//        public override GetLevyDeclarationQueryHandler RequestHandler { get; set; }
//        public override Mock<IValidator<GetLevyDeclarationRequest>> RequestValidator { get; set; }
//        private const long ExpectedAccountId = 757361;
//        private List<LevyDeclarationView> _expectedLevyDeclarationViews;
        
//        [SetUp]
//        public void Arrange()
//        {
//            SetUp();

//            Query = new GetLevyDeclarationRequest { HashedAccountId = "ABC123" };

//            _hashingService = new Mock<IHashingService>();
//            _hashingService.Setup(x => x.DecodeValue(Query.HashedAccountId)).Returns(ExpectedAccountId);
            
//            _repository = new Mock<IDasLevyRepository>();
//            _expectedLevyDeclarationViews = LevyDeclarationViewsObjectMother.Create(ExpectedAccountId);
//            _repository.Setup(x => x.GetAccountLevyDeclarations(It.IsAny<long>())).ReturnsAsync(_expectedLevyDeclarationViews);

//           RequestHandler = new GetLevyDeclarationQueryHandler(_repository.Object, RequestValidator.Object, _hashingService.Object);
//        }

//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
//        {
//            //Act
//            await RequestHandler.Handle(Query);

//            //Assert
//            _repository.Verify(x=>x.GetAccountLevyDeclarations(ExpectedAccountId), Times.Once);
//        }

//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
//        {
//            //Act
//            var actual = await RequestHandler.Handle(Query);

//            //Assert
//            Assert.IsNotNull(actual);
//            Assert.AreSame(_expectedLevyDeclarationViews, actual.Declarations);
//        }
//    }
//}
