using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.TestCommon.ObjectMothers;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLevyDeclarationTests
{
    public class WhenIGetLevyDeclarations : QueryBaseTest<GetLevyDeclarationQueryHandler,GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private Mock<IDasLevyRepository> _repository;
        public override GetLevyDeclarationRequest Query { get; set; }
        public override GetLevyDeclarationQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetLevyDeclarationRequest>> RequestValidator { get; set; }
        private const long ExpectedAccountId = 757361;
        private const string ExpectedEmpref = "123/ABC123";
        private List<LevyDeclarationView> _expectedLevyDeclarationViews;
        private Mock<IDasAccountService> _dasAccountService;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetLevyDeclarationRequest {AccountId = ExpectedAccountId};
            
            _repository = new Mock<IDasLevyRepository>();
            _expectedLevyDeclarationViews = LevyDeclarationViewsObjectMother.Create(ExpectedAccountId);
            _repository.Setup(x => x.GetAccountLevyDeclarations(It.IsAny<long>())).ReturnsAsync(_expectedLevyDeclarationViews);

            _dasAccountService = new Mock<IDasAccountService>();
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new PayeSchemes {SchemesList = new List<PayeScheme>
            {
                new PayeScheme
                {
                    Ref = ExpectedEmpref
                }
            } });

            RequestHandler = new GetLevyDeclarationQueryHandler(_repository.Object, RequestValidator.Object, _dasAccountService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetAccountLevyDeclarations(ExpectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_expectedLevyDeclarationViews[0].AccountId,actual.Data.AccountId);
            Assert.AreEqual(_expectedLevyDeclarationViews[0].EmpRef,actual.Data.Data[0].EmpRef);
            Assert.AreEqual(_expectedLevyDeclarationViews[0].EnglishFraction,actual.Data.Data[0].EnglishFraction);
            Assert.AreEqual(_expectedLevyDeclarationViews[0].SubmissionDate,actual.Data.Data[0].SubmissionDate);
        }

        [Test]
        public async Task ThenTheSchemeInformationIsRetrieved()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasAccountService.Verify(x=>x.GetAccountSchemes(ExpectedAccountId));
        }

        [Test]
        public async Task ThenTheSchemeInformationIsPopulatedForTheResponse()
        {
            //Arrange
            var expectedAddedDate = new DateTime(2016,01,10);
            var expectedRemovedDate = new DateTime(2016,03,29);
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new PayeSchemes { SchemesList = new List<PayeScheme> {
                new PayeScheme
                    {
                        AddedDate = expectedAddedDate,
                        RemovedDate = expectedRemovedDate,
                        Ref = ExpectedEmpref
                    }
                }
            });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(expectedAddedDate, actual.Data.Data[0].EmprefAddedDate);
            Assert.AreEqual(expectedRemovedDate, actual.Data.Data[0].EmprefRemovedDate);
        }
    }
}
