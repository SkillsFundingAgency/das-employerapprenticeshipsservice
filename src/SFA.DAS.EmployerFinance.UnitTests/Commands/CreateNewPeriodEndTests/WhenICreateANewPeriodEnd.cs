﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Exceptions;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.CreateNewPeriodEndTests
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
            var command = new CreateNewPeriodEndCommand { NewPeriodEnd = new PeriodEnd { CalendarPeriodMonth = 1, CalendarPeriodYear = 1 } };
            
            //Act
            await _handler.Handle(command);

            //Assert
            _repository.Verify(x=>x.CreateNewPeriodEnd(It.IsAny<PeriodEnd>()));
        }
    }
}
