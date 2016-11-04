using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateNewPeriodEndTests
{
    public class WhenICreateANewPeriodEnd
    {
        private CreateNewPeriodEndCommandHandler _handler;
        private Mock<IValidator<CreateNewPeriodEndCommand>> _validator;
        private Mock<IDasLevyRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IDasLevyRepository>();

            _validator = new Mock<IValidator<CreateNewPeriodEndCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateNewPeriodEndCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> ()});

            _handler = new CreateNewPeriodEndCommandHandler(_validator.Object, _repository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _handler.Handle(new CreateNewPeriodEndCommand());

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<CreateNewPeriodEndCommand>()),Times.Once);

        }

        [Test]
        public void ThenAnInvalidRequestionExceptionIsThrownWhenTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<CreateNewPeriodEndCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { {"",""} } });

            //Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new CreateNewPeriodEndCommand()));
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledWhenTheMessageIsValid()
        {
            //Arrange
            var command = new CreateNewPeriodEndCommand { NewPeriodEnd = new PeriodEnd { CalendarPeriod = new CalendarPeriod { Month = 1, Year = 1 } } };
            
            //Act
            await _handler.Handle(command);

            //Assert
            _repository.Verify(x=>x.CreateNewPeriodEnd(It.IsAny<PeriodEnd>()));
            
        }
    }
}
