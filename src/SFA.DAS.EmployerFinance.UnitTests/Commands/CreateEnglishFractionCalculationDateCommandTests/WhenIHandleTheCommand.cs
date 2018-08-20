﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Exceptions;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.CreateEnglishFractionCalculationDateCommandTests
{
    public class WhenIHandleTheCommand
    {
        private CreateEnglishFractionCalculationDateCommandHandler _handler;
        private Mock<IValidator<CreateEnglishFractionCalculationDateCommand>> _validator;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;
        private Mock<ILog> _logger;
        private DateTime _expectedDate;

        [SetUp]
        public void Arrange()
        {
            _expectedDate = new DateTime(2016, 10, 30);

            _validator = new Mock<IValidator<CreateEnglishFractionCalculationDateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateEnglishFractionCalculationDateCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _englishFractionRepository = new Mock<IEnglishFractionRepository>();

            _logger = new Mock<ILog>();

            _handler = new CreateEnglishFractionCalculationDateCommandHandler(_validator.Object, _englishFractionRepository.Object, _logger.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAInvalidRequestExceptionIsThrownIfNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<CreateEnglishFractionCalculationDateCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async ()=> await _handler.Handle(new CreateEnglishFractionCalculationDateCommand()));

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<CreateEnglishFractionCalculationDateCommand>()),Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsUpdatedWhenTheCommandIsValid()
        {
            //Act
            await _handler.Handle(new CreateEnglishFractionCalculationDateCommand {DateCalculated = _expectedDate});

            //Assert
            _englishFractionRepository.Verify(x=>x.SetLastUpdateDate(_expectedDate));
        }

        [Test]
        public async Task ThenAnInfoLevelMessageIsLoggedWhenItHasBeenUpdated()
        {
            //Act
            await _handler.Handle(new CreateEnglishFractionCalculationDateCommand { DateCalculated = _expectedDate });

            //Assert
            _logger.Verify(x => x.Info($"English Fraction CalculationDate updated to {_expectedDate.ToString("dd MMM yyyy")}"), Times.Once());
        }
    }
}
