using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdatePayeInformation;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdatePayeInformationTests
{
    public class WhenUpdatingPayeInformation
    {
        private UpdatePayeInformationCommandHandler _handler;
        private Mock<IValidator<UpdatePayeInformationCommand>> _validator;
        private Mock<IPayeRepository> _payeRepository;
        private Mock<IHmrcService> _hmrcService;
        private const string ExpectedEmpRef = "123RFV";
        private const string ExpectedEmpRefName = "Test Scheme Name";
        
        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpdatePayeInformationCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UpdatePayeInformationCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> () });

            _payeRepository = new Mock<IPayeRepository>();
            _payeRepository.Setup(x => x.GetPayeSchemeByRef(ExpectedEmpRef)).ReturnsAsync(new Paye { EmpRef = ExpectedEmpRef });

            _hmrcService = new Mock<IHmrcService>();
            _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedEmpRef))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer {Name = new Name {EmprefAssociatedName = ExpectedEmpRefName}}
                });

            _handler = new UpdatePayeInformationCommandHandler(_validator.Object, _payeRepository.Object, _hmrcService.Object);
        }

        [Test]
        public void ThenTheCommandIsValidatedAndAnInvalidRequestExceptionIsThrownIfNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UpdatePayeInformationCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async ()=> await _handler.Handle(new UpdatePayeInformationCommand()));

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<UpdatePayeInformationCommand>()));
        }

        [Test]
        public async Task ThenTheSchemeIsRetrievedFromTheDatabase()
        {
            //Act
            await _handler.Handle(new UpdatePayeInformationCommand {PayeRef = ExpectedEmpRef});

            //Assert
            _payeRepository.Verify(x=>x.GetPayeSchemeByRef(ExpectedEmpRef),Times.Once);
        }

        [Test]
        public async Task ThenIfTheSchemeReturnedHasNoNameAssociatedWithItThenTheHmrcServiceIsCalled()
        {
            //Arrange
            _payeRepository.Setup(x => x.GetPayeSchemeByRef(ExpectedEmpRef)).ReturnsAsync(new Paye {EmpRef = ExpectedEmpRef});

            //Act
            await _handler.Handle(new UpdatePayeInformationCommand { PayeRef = ExpectedEmpRef });

            //Assert
            _hmrcService.Verify(x => x.GetEmprefInformation(ExpectedEmpRef), Times.Once);
        }

        [Test]
        public async Task ThenIftheScehmeReturnedHasANameThenTheHmrcServiceIsNotCalled()
        {
            //Arrange
            _payeRepository.Setup(x => x.GetPayeSchemeByRef(ExpectedEmpRef)).ReturnsAsync(new Paye { EmpRef = ExpectedEmpRef, RefName = "Test" });

            //Act
            await _handler.Handle(new UpdatePayeInformationCommand { PayeRef = ExpectedEmpRef });

            //Assert
            _hmrcService.Verify(x=>x.GetEmprefInformation(It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenIfTheSchemeNameIsPopulatedTheRecordIsUpdated()
        {
            //Act
            await _handler.Handle(new UpdatePayeInformationCommand { PayeRef = ExpectedEmpRef });

            //Assert
            _payeRepository.Verify(x=>x.UpdatePayeSchemeName(ExpectedEmpRef,ExpectedEmpRefName), Times.Once);
        }


        [Test]
        public async Task ThenIfTheSchemeNameIsNotPopulatedTheRecordIsNotUpdated()
        {
            //Arrange
            _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedEmpRef))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer { Name = new Name () }
                });

            //Act
            await _handler.Handle(new UpdatePayeInformationCommand { PayeRef = ExpectedEmpRef });

            //Assert
            _payeRepository.Verify(x => x.UpdatePayeSchemeName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIfNullIsReturnedFromHmrcThenTheRecordIsNotUpdated()
        {
            //Arrange
            _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedEmpRef)).ReturnsAsync(null);

            //Act
            await _handler.Handle(new UpdatePayeInformationCommand { PayeRef = ExpectedEmpRef });

            //Assert
            _payeRepository.Verify(x => x.UpdatePayeSchemeName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
