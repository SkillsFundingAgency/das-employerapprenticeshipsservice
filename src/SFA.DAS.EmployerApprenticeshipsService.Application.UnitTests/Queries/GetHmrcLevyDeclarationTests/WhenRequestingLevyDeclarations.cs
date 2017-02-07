using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.TestCommon.ObjectMothers;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetHmrcLevyDeclarationTests
{
    public class WhenRequestingLevyDeclarations
    {
        private const string ExpectedEmpRef = "12345";
        private GetHMRCLevyDeclarationQueryHandler _getHMRCLevyDeclarationQueryHandler;
        private Mock<IValidator<GetHMRCLevyDeclarationQuery>> _validator;
        private Mock<IHmrcService> _hmrcService;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetHMRCLevyDeclarationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetHMRCLevyDeclarationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _hmrcService = new Mock<IHmrcService>();
            _hmrcService.Setup(x => x.GetEnglishFractions(ExpectedEmpRef)).ReturnsAsync(EnglishFractionObjectMother.Create(ExpectedEmpRef));
            _hmrcService.Setup(x => x.GetLevyDeclarations(ExpectedEmpRef)).ReturnsAsync(DeclarationsObjectMother.Create(ExpectedEmpRef));
            
            _getHMRCLevyDeclarationQueryHandler = new GetHMRCLevyDeclarationQueryHandler(_validator.Object, _hmrcService.Object);
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
            await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { EmpRef = ExpectedEmpRef });

            //Assert
            _hmrcService.Verify(x => x.GetLevyDeclarations(It.Is<string>(c => c.Equals(ExpectedEmpRef))), Times.Once);
        }
        

        [Test]
        public async Task ThenTheResponseIsPopulatedWithDeclarations()
        {
            //Act
            var actual = await _getHMRCLevyDeclarationQueryHandler.Handle(new GetHMRCLevyDeclarationQuery { EmpRef = ExpectedEmpRef });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedEmpRef, actual.Empref);
            Assert.IsTrue(actual.LevyDeclarations.Declarations.Any());
        }
    }
}
