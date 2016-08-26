using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetHmrcLevyDeclarationTests
{
    public class WhenRequestingLevyDeclarations
    {
        private const string ExpectedEmpRef = "12345";
        private const string AuthToken = "123";
        private GetHMRCLevyDeclarationQueryHandler _getHMRCLevyDeclarationQueryHandler;
        private Mock<IValidator<GetHMRCLevyDeclarationQuery>> _validator;
        private Mock<IHmrcService> _hmrcService;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetHMRCLevyDeclarationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetHMRCLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _hmrcService = new Mock<IHmrcService>();
            _hmrcService.Setup(x => x.GetEnglishFraction(It.IsAny<string>(), ExpectedEmpRef)).ReturnsAsync(EnglishFractionObjectMother.Create(ExpectedEmpRef));
            _hmrcService.Setup(x => x.GetLevyDeclarations(It.IsAny<string>(), ExpectedEmpRef)).ReturnsAsync(DeclarationsObjectMother.Create(ExpectedEmpRef));

            _englishFractionRepository = new Mock<IEnglishFractionRepository>();

            _getHMRCLevyDeclarationQueryHandler = new GetHMRCLevyDeclarationQueryHandler(_validator.Object, _hmrcService.Object, _englishFractionRepository.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<GetHMRCLevyDeclarationQuery>()));
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheQueryIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<GetHMRCLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery()));
        }

        [Test]
        public async Task ThenTheLevyServiceIsCalledWithThePassedIdToGetTheLevyDeclarations()
        {
            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            _hmrcService.Verify(x => x.GetLevyDeclarations(AuthToken, It.Is<string>(c => c.Equals(ExpectedEmpRef))), Times.Once);
        }

        [Test]
        public async Task ThenTheLevyServiceIsCalledWithThePassedIdToGetTheFractions()
        {
            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            _hmrcService.Verify(x => x.GetEnglishFraction(AuthToken, It.Is<string>(c => c.Equals(ExpectedEmpRef))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseIsPopulatedWithDeclarations()
        {
            //Act
            var actual = await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedEmpRef, actual.Empref);
            Assert.IsTrue(actual.LevyDeclarations.Declarations.Any());
            Assert.IsTrue(actual.Fractions.FractionCalculations.Any());
        }

        [Test]
        public async Task ThenIShouldNotCallHrmcForAEnglishFractionsUpdateIfIhaveTheLastest()
        {
            //Assign
            var fraction = new DasEnglishFraction {DateCalculated = DateTime.Now};
            _englishFractionRepository.Setup(x => x.GetLatest(ExpectedEmpRef)).ReturnsAsync(fraction);
            _hmrcService.Setup(x => x.GetLastEnglishFractionUpdate()).ReturnsAsync(DateTime.Now.AddDays(-1));
            _hmrcService.Setup(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>()));
            
            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            _englishFractionRepository.Verify(x => x.GetLatest(ExpectedEmpRef), Times.Once);
            _hmrcService.Verify(x => x.GetLastEnglishFractionUpdate(), Times.Once);
            _hmrcService.Verify(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldCallHrmcForAEnglishFractionsUpdateIfIHaveAnOutOfDateFractionValue()
        {
            //Assign
            var fraction = new DasEnglishFraction { DateCalculated = DateTime.Now.AddDays(-1) };
            _englishFractionRepository.Setup(x => x.GetLatest(ExpectedEmpRef)).ReturnsAsync(fraction);
            _hmrcService.Setup(x => x.GetLastEnglishFractionUpdate()).ReturnsAsync(DateTime.Now);
            _hmrcService.Setup(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EnglishFractionDeclarations());

            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            _englishFractionRepository.Verify(x => x.GetLatest(ExpectedEmpRef), Times.Once);
            _hmrcService.Verify(x => x.GetLastEnglishFractionUpdate(), Times.Once);
            _hmrcService.Verify(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenIShouldCallHrmcForAEnglishFractionsUpdateAndSaveTheNewFractionRetrievedFromHmrc()
        {
            //Assign
            var fraction = new DasEnglishFraction { DateCalculated = DateTime.Now.AddDays(-1) };
            var expectedSavedFraction = new DasEnglishFraction
            {
                EmpRef = ExpectedEmpRef,
                DateCalculated = DateTime.Now,
                Amount = 0.80M,
            };

            var fractionCalculation = new FractionCalculation
            {
                CalculatedAt = expectedSavedFraction.DateCalculated.ToShortDateString(),
                Fractions = new List<Fraction>
                {
                    new Fraction {Region = "England", Value = expectedSavedFraction.Amount.ToString("#.##")}
                }
            };

            _englishFractionRepository.Setup(x => x.GetLatest(ExpectedEmpRef)).ReturnsAsync(fraction);
            _englishFractionRepository.Setup(x => x.Save(It.IsAny<DasEnglishFraction>()));
            _hmrcService.Setup(x => x.GetLastEnglishFractionUpdate()).ReturnsAsync(DateTime.Now);
            _hmrcService.Setup(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
                new EnglishFractionDeclarations
                {
                    Empref = ExpectedEmpRef,
                    FractionCalculations = new List<FractionCalculation> { fractionCalculation }
                });

            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { AuthToken = AuthToken, EmpRef = ExpectedEmpRef });

            //Assert
            _englishFractionRepository.Verify(x => x.GetLatest(ExpectedEmpRef), Times.Once);
            _englishFractionRepository.Verify(x => x.Save(
                It.Is<DasEnglishFraction>( englishFraction => 
                englishFraction.DateCalculated.ToShortDateString().Equals(expectedSavedFraction.DateCalculated.ToShortDateString()) && 
                englishFraction.EmpRef.Equals(expectedSavedFraction.EmpRef) &&
                englishFraction.Amount.Equals(expectedSavedFraction.Amount))));

            _hmrcService.Verify(x => x.GetLastEnglishFractionUpdate(), Times.Once);
            _hmrcService.Verify(x => x.GetEnglishFraction(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
