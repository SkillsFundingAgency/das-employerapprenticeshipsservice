using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetHmrcEmployerInformation
{
    public class WhenRequestingEmployerInformation
    {
        private GetHmrcEmployerInformationHandler _getHmrcEmployerInformationHandler;
        private Mock<IValidator<GetHmrcEmployerInformationQuery>> _validator;
        private Mock<IHmrcService> _hmrcService;

        [SetUp]
        public void Arrange()
        {
            _hmrcService = new Mock<IHmrcService>();
            _validator = new Mock<IValidator<GetHmrcEmployerInformationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _getHmrcEmployerInformationHandler = new GetHmrcEmployerInformationHandler(_validator.Object, _hmrcService.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>()), Times.Once);

        }

        [Test]
        public void ThenTheHmrcServiceIsNotCalledIfTheMessageIsNotValidAnAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery()));

            //Assert
            _hmrcService.Verify(x => x.GetEmprefInformation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheHmrcServiceIsCalledWithTheCorrectData()
        {
            //Arrange
            var expectedAuthToken = "token1";
            var expectedEmpref = "123/avf123";

            //Act
            await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = expectedAuthToken, Empref = expectedEmpref });

            //Assert
            _hmrcService.Verify(x => x.GetEmprefInformation(expectedAuthToken, expectedEmpref), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseReturnsThePopulatedHmrcEmployerInformation()
        {
            //Arrange
            var expectedAuthToken = "token1";
            var expectedEmpref = "123/avf123";
            var expectedEmprefAssociatedName = "test company";
            _hmrcService.Setup(x => x.GetEmprefInformation(expectedAuthToken, expectedEmpref)).ReturnsAsync(new EmpRefLevyInformation { Employer = new Employer { Name = new Name { EmprefAssociatedName = expectedEmprefAssociatedName } }, Links = new Links() });

            //Act
            var actual = await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = expectedAuthToken, Empref = expectedEmpref });

            //Assert
            Assert.AreEqual(expectedEmprefAssociatedName, actual.EmployerLevyInformation.Employer.Name.EmprefAssociatedName);
        }
    }
}
