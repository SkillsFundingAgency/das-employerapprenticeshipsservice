using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetHmrcEmployerInformationTests
{
    public class WhenRequestingEmployerInformation
    {
        private GetHmrcEmployerInformationHandler _getHmrcEmployerInformationHandler;
        private Mock<IValidator<GetHmrcEmployerInformationQuery>> _validator;
        private Mock<IHmrcService> _hmrcService;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private const string ExpectedAuthToken = "token1";
        private const string ExpectedEmpref = "123/avf123";
        private const string ExpectedEmprefAssociatedName = "test company";

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _hmrcService = new Mock<IHmrcService>();
            _hmrcService.Setup(x => x.DiscoverEmpref(ExpectedAuthToken)).ReturnsAsync(ExpectedEmpref);
            _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedAuthToken, ExpectedEmpref)).ReturnsAsync(new EmpRefLevyInformation { Employer = new Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefAssociatedName } }, Links = new Links() });

            _validator = new Mock<IValidator<GetHmrcEmployerInformationQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _mediator = new Mock<IMediator>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration {Hmrc = new HmrcConfiguration {IgnoreDuplicates = false} };

            _getHmrcEmployerInformationHandler = new GetHmrcEmployerInformationHandler(_validator.Object, _hmrcService.Object, _mediator.Object, _logger.Object, _configuration);
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
            //Act
            await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken });

            //Assert
            _hmrcService.Verify(x => x.GetEmprefInformation(ExpectedAuthToken, ExpectedEmpref), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseReturnsThePopulatedHmrcEmployerInformation()
        {
            //Act
            var actual = await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken});

            //Assert
            Assert.AreEqual(ExpectedEmprefAssociatedName, actual.EmployerLevyInformation.Employer.Name.EmprefAssociatedName);
            Assert.AreEqual(ExpectedEmpref, actual.Empref);
        }

        [Test]
        public async Task ThenTheSuccesfulResponseIsCheckedToSeeIfItIsAlreadyRegistered()
        {
            
            //Act
            await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken });

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetPayeSchemeInUseQuery>(c=>c.Empref.Equals(ExpectedEmpref))));
        }

        [Test]
        public void ThenTheMessageIsNotValidIfTheSchemeAlreadyExists()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse {PayeScheme = new Scheme()});

            //Act
            Assert.ThrowsAsync<ConstraintException>(async ()=> await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken }));
            _logger.Verify(x=>x.Warn($"PAYE scheme {ExpectedEmpref} already in use."), Times.Once);
        }

        [Test]
        public async Task ThenTheConstraintIsNotThrownIfTheConfigIsSetToIgnoreDuplicates()
        {
            //Arrange
            _configuration = new EmployerApprenticeshipsServiceConfiguration { Hmrc = new HmrcConfiguration { IgnoreDuplicates = true } };
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = new Scheme() });
            _getHmrcEmployerInformationHandler = new GetHmrcEmployerInformationHandler(_validator.Object, _hmrcService.Object, _mediator.Object, _logger.Object, _configuration);

            //Act
            var actual = await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken });

            //Assert
            Assert.IsNotNull(actual);
        }
    }
}
