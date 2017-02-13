using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLastLevyDeclarationsTests
{
    public class WhenIGetLevyDeclarations
    {
        private GetLastLevyDeclarationRequestHandler _handler;
        private Mock<IValidator<GetLastLevyDeclarationRequest>> _validator;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        
        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetLastLevyDeclarationRequest>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetLastLevyDeclarationRequest>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            
            _handler = new GetLastLevyDeclarationRequestHandler(_validator.Object, _dasLevyRepository.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalledToMakeSureTheRequestIsValid()
        {
            //Act
            await _handler.Handle(new GetLastLevyDeclarationRequest());

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<GetLastLevyDeclarationRequest>()),Times.Once);

        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheRequestIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<GetLastLevyDeclarationRequest>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new GetLastLevyDeclarationRequest()));

        }

        [Test]
        public async Task ThenIfTheRequestIsValidTheRepositoryIsCalledWithThePassedParameter()
        {
            //Arrange
            var expectedEmpref = "45TGB";

            //Act
            await _handler.Handle(new GetLastLevyDeclarationRequest {Empref = expectedEmpref});

            //Assert
            _dasLevyRepository.Verify(x=>x.GetLastSubmissionForScheme(expectedEmpref));
        }

        [Test]
        public async Task ThenIfTheRequestIsValidThenTheDataIsReturnedInTheResponse()
        {
            //Arrange
            var expectedEmpref = "45TGB";
            var expectedDate = new DateTime(2016, 01, 29);
            _dasLevyRepository.Setup(x => x.GetLastSubmissionForScheme(expectedEmpref)).ReturnsAsync(new DasDeclaration {Date = expectedDate});

            //Act
            var actual = await _handler.Handle(new GetLastLevyDeclarationRequest { Empref = expectedEmpref });

            //Assert
            Assert.IsNotNull(actual.Transaction);
            Assert.AreEqual(expectedDate, actual.Transaction.Date);
        }
    }
}
