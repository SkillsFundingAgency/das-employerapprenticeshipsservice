using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetLevyDeclarationTests
{
    public class WhenRequestingLevyDeclarations
    {
        private const string ExpectedEmployerId = "12345";
        private GetHMRCLevyDeclarationQueryHandler _getHMRCLevyDeclarationQueryHandler;
        private Mock<IValidator<GetHMRCLevyDeclarationQuery>> _validator;
        private Mock<ILevyDeclarationService> _levyDeclarationService;


        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetHMRCLevyDeclarationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetHMRCLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _levyDeclarationService = new Mock<ILevyDeclarationService>();
            _levyDeclarationService.Setup(x => x.GetEnglishFraction(ExpectedEmployerId)).ReturnsAsync(EnglishFractionObjectMother.Create(ExpectedEmployerId));
            _levyDeclarationService.Setup(x => x.GetLevyDeclarations(ExpectedEmployerId)).ReturnsAsync(DeclarationsObjectMother.Create(ExpectedEmployerId));

            _getHMRCLevyDeclarationQueryHandler = new GetHMRCLevyDeclarationQueryHandler(_validator.Object, _levyDeclarationService.Object);
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
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            _levyDeclarationService.Verify(x => x.GetLevyDeclarations(It.Is<string>(c => c.Equals(ExpectedEmployerId))), Times.Once);
        }

        [Test]
        public async Task ThenTheLevyServiceIsCalledWithThePassedIdToGetTheFractions()
        {
            //Act
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            _levyDeclarationService.Verify(x => x.GetEnglishFraction(It.Is<string>(c => c.Equals(ExpectedEmployerId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseIsPopulatedWithDeclarations()
        {
            //Act
            var actual = await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedEmployerId, actual.Empref);
            Assert.IsTrue(actual.Declarations.declarations.Any());
            Assert.IsTrue(actual.Fractions.FractionCalculations.Any());

        }

    }
}
