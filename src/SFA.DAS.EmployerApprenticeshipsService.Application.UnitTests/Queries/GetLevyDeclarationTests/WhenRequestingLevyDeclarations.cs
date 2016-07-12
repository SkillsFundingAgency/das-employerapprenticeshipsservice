using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Application.Tests.ObjectMothers;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Queries.GetLevyDeclarationTests
{
    public class WhenRequestingLevyDeclarations
    {
        private const string ExpectedEmployerId = "12345";
        private GetLevyDeclarationQueryHandler _getLevyDeclarationQueryHandler;
        private Mock<IValidator<GetLevyDeclarationQuery>> _validator;
        private Mock<ILevyDeclarationService> _levyDeclarationService;


        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetLevyDeclarationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _levyDeclarationService = new Mock<ILevyDeclarationService>();
            _levyDeclarationService.Setup(x => x.GetEnglishFraction(ExpectedEmployerId)).ReturnsAsync(EnglishFractionObjectMother.Create(ExpectedEmployerId));
            _levyDeclarationService.Setup(x => x.GetLevyDeclarations(ExpectedEmployerId)).ReturnsAsync(DeclarationsObjectMother.Create(ExpectedEmployerId));

            _getLevyDeclarationQueryHandler = new GetLevyDeclarationQueryHandler(_validator.Object, _levyDeclarationService.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _getLevyDeclarationQueryHandler.Handle(new GetLevyDeclarationQuery());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<GetLevyDeclarationQuery>()));
        }

        [Test]
        public async Task ThenNullIsReturnedIfTheQueryIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<GetLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            var actual = await _getLevyDeclarationQueryHandler.Handle(new GetLevyDeclarationQuery());

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenTheLevyServiceIsCalledWithThePassedIdToGetTheLevyDeclarations()
        {
            //Act
            await _getLevyDeclarationQueryHandler.Handle(new GetLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            _levyDeclarationService.Verify(x => x.GetLevyDeclarations(It.Is<string>(c => c.Equals(ExpectedEmployerId))), Times.Once);
        }

        [Test]
        public async Task ThenTheLevyServiceIsCalledWithThePassedIdToGetTheFractions()
        {
            //Act
            await _getLevyDeclarationQueryHandler.Handle(new GetLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            _levyDeclarationService.Verify(x => x.GetEnglishFraction(It.Is<string>(c => c.Equals(ExpectedEmployerId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseIsPopulatedWithDeclarations()
        {
            //Act
            var actual = await _getLevyDeclarationQueryHandler.Handle(new GetLevyDeclarationQuery { Id = ExpectedEmployerId });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedEmployerId, actual.Empref);
            Assert.IsTrue(actual.Declarations.declarations.Any());
            Assert.IsTrue(actual.Fractions.FractionCalculations.Any());

        }

    }
}
